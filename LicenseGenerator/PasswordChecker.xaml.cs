using System;
using System.ComponentModel;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LicenseGenerator
{
    /// <summary>
    /// Interaction logic for PasswordChecker.xaml
    /// </summary>
    public partial class PasswordCheckerDialogBox : Window
    {
        const string correctPassword = "09210Jms";
        string PasswordFromUser_ = "";
        public bool IsPasswordCorrect
        {
            get
            {
                if (PasswordFromUser_ == correctPassword) return true;
                else return false;
            }
        }
        
        PasswordCheckerDialogBoxViewModel vm_;

        public PasswordCheckerDialogBox()
        {
            InitializeComponent();
            if (vm_ == null) vm_ = new PasswordCheckerDialogBoxViewModel(Ok_Clicked);
            this.DataContext = vm_;
        }
        public void Ok_Clicked(string passwordFromUser)
        {
            this.PasswordFromUser_ = passwordFromUser;
            this.DialogResult = true;
            this.Close();
        }
    }
    public class PasswordCheckerDialogBoxViewModel: INotifyPropertyChanged
    {
        bool canExecute_;
        Action<string> updatePassword_;

        public PasswordCheckerDialogBoxViewModel(Action<string> updatePassword)
        {
            this.canExecute_ = true;
            this.updatePassword_ = updatePassword;
        }

        ICommand OKCommand_;
        public ICommand OKCommand
        {
            get { return OKCommand_ ?? (OKCommand_ = new CommandHandler(updatePassword_, canExecute_)); }
        }
         

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class CommandHandler : ICommand
    {
        bool canExecute_;
        public event EventHandler CanExecuteChanged;
        Action<string> actionUpdatePassword_;

        public CommandHandler(Action<string> actionUpdatePassword_, bool canExecute)
        {
            this.actionUpdatePassword_ = actionUpdatePassword_;
            this.canExecute_ = canExecute;
        }
        public bool CanExecute(object parameter)
        {
            return canExecute_;
        }

        public void Execute(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox.Password;
            actionUpdatePassword_.Invoke(password);
        }
    }

}
