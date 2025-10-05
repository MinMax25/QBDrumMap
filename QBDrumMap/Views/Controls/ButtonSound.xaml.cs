using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QBDrumMap.Views.Controls
{
    public partial class ButtonSound : UserControl
    {
        private bool _isPressed;

        public ButtonSound()
        {
            InitializeComponent();

            PlayButton.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            PlayButton.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
        }

        #region Dependency Properties

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(ButtonSound), new PropertyMetadata(null));
        public object CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

        public static readonly DependencyProperty NoteOnCommandProperty =
            DependencyProperty.Register(nameof(NoteOnCommand), typeof(ICommand), typeof(ButtonSound));
        public ICommand NoteOnCommand { get => (ICommand)GetValue(NoteOnCommandProperty); set => SetValue(NoteOnCommandProperty, value); }

        public static readonly DependencyProperty NoteOffCommandProperty =
            DependencyProperty.Register(nameof(NoteOffCommand), typeof(ICommand), typeof(ButtonSound));
        public ICommand NoteOffCommand { get => (ICommand)GetValue(NoteOffCommandProperty); set => SetValue(NoteOffCommandProperty, value); }

        #endregion

        #region Mouse Events

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isPressed) return;

            _isPressed = true;

            Mouse.Capture(PlayButton);

            if (NoteOnCommand?.CanExecute(CommandParameter) == true)
            {
                NoteOnCommand.Execute(CommandParameter);
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isPressed) return;

            _isPressed = false;
            Mouse.Capture(null);

            if (NoteOffCommand?.CanExecute(CommandParameter) == true)
            {
                NoteOffCommand.Execute(CommandParameter);
            }
        }

        #endregion
    }
}
