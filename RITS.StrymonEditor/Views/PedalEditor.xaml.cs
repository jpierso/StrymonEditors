﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;
using RITS.StrymonEditor.Messaging;
namespace RITS.StrymonEditor.Views
{
    /// <summary>
    /// Interaction logic for TimelineEditor.xaml
    /// </summary>
    public partial class PedalEditor : Window, INotifyPropertyChanged
    {
        private StrymonPreset editingPreset;
        private IStrymonMidiManager midiManager;
        public PedalEditor(StrymonPreset preset, IStrymonMidiManager midiManager)
        {
            editingPreset = preset;
            this.midiManager = midiManager;
            InitializeComponent();                        
        }

        #region INotify
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private StrymonPedalViewModel _pedalViewModel;
        public StrymonPedalViewModel PedalViewModel
        {
            get
            {
                if (_pedalViewModel == null) 
                {
                    _pedalViewModel = new StrymonPedalViewModel(editingPreset, midiManager);
                    _pedalViewModel.CloseWindow = CloseMe;
                }
                
                return _pedalViewModel;
            }
            set
            {
                if (value == null) return;
                // Dereference messages etc
                _pedalViewModel.Dispose();
                _pedalViewModel = null;
                _pedalViewModel = value;
                OnPropertyChanged("PedalViewModel");
            }
        }

        private void CloseMe()
        {
            this.Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (PedalViewModel.IsDirty)
            {                
                MessageBoxResult result = MessageBox.Show("There are unsaved edits, do you wish to close?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
            Globals.PotValueMap = null;
            PedalViewModel.Dispose();
            PedalViewModel = null;
        }

    }
}
