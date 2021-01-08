using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Labyrinth.Behaviors {
    public class SetClassAction : AvaloniaObject, IAction {
        public string ClassName {
            get => GetValue(ClassNameProperty);
            set => SetValue(ClassNameProperty, value);
        }

        public static readonly StyledProperty<string> ClassNameProperty =
            AvaloniaProperty.Register<SetClassAction, string>(nameof(ClassName));

        public bool IsEnabled {
            get => GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        public static readonly StyledProperty<bool> IsEnabledProperty =
            AvaloniaProperty.Register<SetClassAction, bool>(nameof(IsEnabled));

        public object? Execute(object? sender, object? _) {
            if (sender is IStyledElement styledElement) {
                styledElement.Classes.Set(ClassName, IsEnabled);
            }

            return null;
        }
    }
}
