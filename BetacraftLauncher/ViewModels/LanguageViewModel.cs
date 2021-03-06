﻿using AutoMapper;
using BetacraftLauncher.Library.Interfaces;
using BetacraftLauncher.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BetacraftLauncher.ViewModels
{
    public class LanguageViewModel : Screen
    {
        private readonly ILanguageEndpoint languageEndpoint;
        private readonly IWindowManager window;
        private readonly IMapper mapper;

        public LanguageViewModel(ILanguageEndpoint languageEndpoint, IWindowManager window, IMapper mapper)
        {
            this.languageEndpoint = languageEndpoint;
            this.window = window;
            this.mapper = mapper;
        }

        private BindingList<LanguageDisplayModel> _languages;

        public BindingList<LanguageDisplayModel> Languages
        {
            get { return _languages; }
            set
            { 
                _languages = value;
                NotifyOfPropertyChange(() => Languages);
            }
        }

        private LanguageDisplayModel _selectedLanguage; 

        public LanguageDisplayModel SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            { 
                _selectedLanguage = value;
                NotifyOfPropertyChange(() => SelectedLanguage);
            }
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            try
            {
                await LoadLanguages();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                await TryCloseAsync();
            }
        }

        private async Task LoadLanguages()
        {
            var languageList = await languageEndpoint.GetLanguages();
            var languages = mapper.Map<List<LanguageDisplayModel>>(languageList);
            Languages = new BindingList<LanguageDisplayModel>(languages);
        }

        public async Task SelectLanguage()
        {
            SaveLanguageSettings();

            await languageEndpoint.DownloadLanguage(SelectedLanguage.Language);

            await TryCloseAsync();
        }

        private void SaveLanguageSettings()
        {
            if (SelectedLanguage != null)
            {
                Properties.Settings.Default.language = SelectedLanguage.Language;
                Properties.Settings.Default.Save();
            }
        }
    }
}
