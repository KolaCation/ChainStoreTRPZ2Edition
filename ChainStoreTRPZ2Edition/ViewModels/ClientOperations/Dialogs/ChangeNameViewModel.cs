using ChainStoreTRPZ2Edition.DataInterfaces;
using ChainStoreTRPZ2Edition.Helpers;
using DevExpress.Mvvm;

namespace ChainStoreTRPZ2Edition.ViewModels.ClientOperations.Dialogs
{
    public sealed class ChangeNameViewModel : ViewModelBase, IVerifiable
    {
        public string Name
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string ErrorMessage
        {
            get => GetValue<string>();
            set => SetValue(value);
        }


        public ChangeNameViewModel(string name)
        {
            Name = name;
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Name))
            {
                ErrorMessage = ErrorMessages.Required(nameof(Name));
                return false;
            }
            else if (Name.Length < 2)
            {
                ErrorMessage = ErrorMessages.StringMinLength(nameof(Name), 2);
                return false;
            }
            else if (Name.Length > 60)
            {
                ErrorMessage = ErrorMessages.StringMaxLength(nameof(Name), 60);
                return false;
            }
            else
            {
                ErrorMessage = string.Empty;
                return true;
            }
        }
    }
}