using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace GeeksWithBlogsToMarkdown.Commands.Base
{
    /// <summary>
    /// This <see cref="System.Windows.Interactivity.TriggerAction" /> can be
    /// used to bind any event on any FrameworkElement to an <see cref="ICommand" />.
    /// Typically, this element is used in XAML to connect the attached element
    /// to a command located in a ViewModel. This trigger can only be attached
    /// to a FrameworkElement or a class deriving from FrameworkElement.
    /// </summary>
    public class EventToCommand : TriggerAction<FrameworkElement>, ICommandSource
    {
        /// <summary>
        /// Identifies the <see cref="CommandParameter" /> dependency property
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter",
            typeof(object),
            typeof(EventToCommand),
            new PropertyMetadata(
                null,
                (s, e) =>
                {
                    var sender = s as EventToCommand;
                    if (sender == null)
                    {
                        return;
                    }

                    if (sender.AssociatedObject == null)
                    {
                        return;
                    }

                    sender.EnableDisableElement();
                }));

        /// <summary>
        /// Identifies the <see cref="Command" /> dependency property
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(EventToCommand),
            new PropertyMetadata(
                null,
                (s, e) => OnCommandChanged(s as EventToCommand, e)));

        /// <summary>
        /// EventArgs
        /// </summary>
        public static readonly DependencyProperty EventArgsProperty = DependencyProperty.Register(
            "EventArgs",
            typeof(object),
            typeof(EventToCommand));

        /// <summary>
        /// Identifies the <see cref="MustToggleIsEnabled" /> dependency property
        /// </summary>
        public static readonly DependencyProperty MustToggleIsEnabledProperty = DependencyProperty.Register(
            "MustToggleIsEnabled",
            typeof(bool),
            typeof(EventToCommand),
            new PropertyMetadata(
                false,
                (s, e) =>
                {
                    var sender = s as EventToCommand;
                    if (sender == null)
                    {
                        return;
                    }

                    if (sender.AssociatedObject == null)
                    {
                        return;
                    }

                    sender.EnableDisableElement();
                }));

        /// <summary>
        /// Gets or sets the ICommand that this trigger is bound to. This
        /// is a DependencyProperty.
        /// </summary>
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }

            set
            {
                SetValue(CommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an object that will be passed to the <see cref="Command" />
        /// attached to this trigger. This is a DependencyProperty.
        /// </summary>
        public object CommandParameter
        {
            get
            {
                return this.GetValue(CommandParameterProperty);
            }

            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        public IInputElement CommandTarget
        {
            get { return this.AssociatedObject; }
        }

        /// <summary>
        /// Allows access to event args in binding
        /// <example>
        /// CommandParameter="{Binding EventArgs, RelativeSource={RelativeSource Self}}"
        /// </example>
        /// </summary>
        public object EventArgs
        {
            get
            {
                return GetValue(EventArgsProperty);
            }

            set
            {
                SetValue(EventArgsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the attached element must be
        /// disabled when the <see cref="Command" /> property's CanExecuteChanged
        /// event fires. If this property is true, and the command's CanExecute
        /// method returns false, the element will be disabled. If this property
        /// is false, the element will not be disabled when the command's
        /// CanExecute method changes. This is a DependencyProperty.
        /// </summary>
        public bool MustToggleIsEnabled
        {
            get
            {
                return (bool)this.GetValue(MustToggleIsEnabledProperty);
            }

            set
            {
                SetValue(MustToggleIsEnabledProperty, value);
            }
        }

        public bool CanExecute()
        {
            if (Command == null)
                return false;

            var routedCommand = Command as RoutedCommand;
            if (routedCommand != null)
                return routedCommand.CanExecute(CommandParameter, CommandTarget);

            return Command.CanExecute(CommandParameter);
        }

        public void Execute()
        {
            if (Command == null)
                return;

            var routedCommand = Command as RoutedCommand;
            if (routedCommand != null)
                routedCommand.Execute(CommandParameter, CommandTarget);
            else
                Command.Execute(CommandParameter);
        }

        /// <summary>
        /// Provides a simple way to invoke this trigger programatically
        /// without any EventArgs.
        /// </summary>
        public void Invoke()
        {
            Invoke(null);
        }

        /// <summary>
        /// Executes the trigger.
        /// <para>To access the EventArgs of the fired event, use a RelayCommand&lt;EventArgs&gt;
        /// and leave the CommandParameter and CommandParameterValue empty!</para>
        /// </summary>
        /// <param name="parameter">The EventArgs of the fired event.</param>
        protected override void Invoke(object parameter)
        {
            EventArgs = parameter;

            if (AssociatedElementIsDisabled())
            {
                return;
            }

            if (CanExecute())
                Execute();
        }

        /// <summary>
        /// Called when this trigger is attached to a FrameworkElement.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            EnableDisableElement();
        }

        private static void OnCommandChanged(
            EventToCommand element,
            DependencyPropertyChangedEventArgs e)
        {
            if (element == null)
            {
                return;
            }

            if (e.OldValue != null)
            {
                ((ICommand)e.OldValue).CanExecuteChanged -= element.OnCommandCanExecuteChanged;
            }

            var command = (ICommand)e.NewValue;

            if (command != null)
            {
                command.CanExecuteChanged += element.OnCommandCanExecuteChanged;
            }

            element.EnableDisableElement();
        }

        private bool AssociatedElementIsDisabled()
        {
            return AssociatedObject != null && !AssociatedObject.IsEnabled;
        }

        private void EnableDisableElement()
        {
            if (AssociatedObject == null)
            {
                return;
            }

            if (MustToggleIsEnabled && Command != null)
            {
                AssociatedObject.IsEnabled = CanExecute();
            }
        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            EnableDisableElement();
        }
    }
}