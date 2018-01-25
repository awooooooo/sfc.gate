﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using SFC.Gate.Models;

namespace SFC.Gate.Material.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        private MainViewModel()
        {
            Messenger.Default.AddListener<string>(Messages.SMS, msg =>
            {
                if (string.IsNullOrEmpty(msg)) return;
                MessageQueue.Enqueue($"SMS Notification: {msg}");
            });
        }

        private ICommand _runExternalCOmmand;

        public ICommand RunExternalCommand => _runExternalCOmmand ?? (_runExternalCOmmand = new DelegateCommand<string>(
        cmd =>
        {
            if (string.IsNullOrWhiteSpace(cmd)) return;
            try
            {
                Process.Start(cmd);
            }
            catch (Exception e)
            {
                //
            }
            
        }));

        private static MainViewModel _instance;
        public static MainViewModel Instance => _instance ?? (_instance = new MainViewModel());
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private User _CurrentUser;

        public User CurrentUser
        {
            get => _CurrentUser;
            set
            {
                if(value == _CurrentUser)
                    return;
                _CurrentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
                OnPropertyChanged(nameof(HasLoggedIn));
                if (value == null)
                    Screen = 5;
            }
        }

        private ICommand _logoutCommand;

        public ICommand LogoutCommand => _logoutCommand ?? (_logoutCommand = new DelegateCommand(d =>
        {
            CurrentUser = null;
        }));
        
        private SnackbarMessageQueue _messageQueue;
        public SnackbarMessageQueue MessageQueue => _messageQueue ?? (_messageQueue = new SnackbarMessageQueue());

        private int _Screen = 5;

        public int Screen
        {
            get => _Screen;
            set
            {
                _Screen = value;
                OnPropertyChanged(nameof(Screen));
            }
        }

        public bool HasLoggedIn => CurrentUser != null;

        public static void ShowMessage(string message,string actionContent, Action action, bool promote = false)
        {
            Instance.MessageQueue.Enqueue(message,actionContent, action, promote);
        }

        public static void ShowMessage<T>(string message, string actionContent, Action<T> action, T param, bool promote = false)
        {
            Instance.MessageQueue.Enqueue(message,actionContent,action,param,promote);
        }

        private ICommand _GeneratePictureCommand;

        public ICommand GeneratePictureCommand =>
            _GeneratePictureCommand ?? (_GeneratePictureCommand = new DelegateCommand(
                d =>
                {
                    var pic = CurrentUser.Picture;
                    CurrentUser?.Update(nameof(User.Picture),Extensions.Generate());
                    ShowMessage("Picture changed","UNDO",()=>CurrentUser.Update("Picture",pic));
                },d=>CurrentUser!=null));

        private bool _ShowUserMenu;

        public bool ShowUserMenu
        {
            get => _ShowUserMenu;
            set
            {
                if(value == _ShowUserMenu)
                    return;
                _ShowUserMenu = value;
                OnPropertyChanged(nameof(ShowUserMenu));
            }
        }

        private int _SettingIndex;

        public int SettingIndex
        {
            get => _SettingIndex;
            set
            {
                _SettingIndex = value;
                OnPropertyChanged(nameof(SettingIndex));
            }
        }

        private ICommand _showUserProfileCommand;

        public ICommand ShowUserProfileCommand =>
            _showUserProfileCommand ?? (_showUserProfileCommand = new DelegateCommand(
                d =>
                {
                    Screen = 4;
                    SettingIndex = 1;
                    
                }));

        private ICommand _showUserMenuCommand;

        public ICommand ShowUserMenuCommand => _showUserMenuCommand ?? (_showUserMenuCommand = new DelegateCommand(d =>
        {
            ShowUserMenu = !ShowUserMenu;
        }));

        private ICommand _showDevCommand;

        public ICommand ShowDevCommand => _showDevCommand ?? (_showDevCommand = new DelegateCommand(d =>
        {
            Screen = 4;
            SettingIndex = 4;
        }));

    }
}
