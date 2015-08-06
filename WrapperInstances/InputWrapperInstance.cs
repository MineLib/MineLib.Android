using System;

using Android.Content;
using Android.Views;
using Android.Views.InputMethods;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using MineLib.Core.Wrappers;


namespace MineLib.Android.WrapperInstances
{
    public class InputWrapperInstance : IInputWrapper
    {
        public event OnKeys OnKey;

        AndroidGameActivity Activity { get; set; }
        View View { get; set; }
        InputMethodManager InputManager { get; set; }


        public InputWrapperInstance(AndroidGameActivity activity, View view, ref Action onBackPressed)
        {
            Activity = activity;
            View = view;

            InputManager = Activity.GetSystemService(Context.InputMethodService) as InputMethodManager;

            if (onBackPressed != null)
                onBackPressed += OnBackPressed;
        }

        private void OnBackPressed()
        {
            if (OnKey != null)
                OnKey((int) Keys.Back);
        }

        public void ShowKeyboard()
        {
            View.RequestFocus();
            View.KeyPress += ViewOnKeyPress;

            if (InputManager != null)
            {
                InputManager.ShowSoftInput(View, ShowFlags.Forced);
                InputManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
            }
        }

        private static int? GetValueOf<T>(string enumConst) where T : struct
        {
            int? result = null;

            T temp;
            if (Enum.TryParse(enumConst, out temp))
                result = Convert.ToInt32(temp);
            
            return result;
        }
        
        private void ViewOnKeyPress(object sender, View.KeyEventArgs keyEventArgs)
        {
            var keyString = keyEventArgs.KeyCode.ToString();
            var result = GetValueOf<Keys>(keyString);

            if (OnKey != null && result.HasValue)
                OnKey(result.Value);
        }

        public void HideKeyboard()
        {
            if (InputManager != null)
                InputManager.HideSoftInputFromWindow(View.WindowToken, HideSoftInputFlags.None);
        }
    }
}
