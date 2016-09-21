using System.Threading;
using System.Windows;

namespace WPFCore.Security.UI
{
    public enum NotInRoleBehaviourEnum
    {
        Disable,
        Hide,
        Collapse
    }

    public static class SecurityManager
    {
        public static DependencyProperty DemandRoleProperty =
            DependencyProperty.RegisterAttached("DemandRole", typeof (string), typeof (SecurityManager),
                new PropertyMetadata(OnDemandRoleChanged));

        public static DependencyProperty NotInRoleBehaviourProperty =
            DependencyProperty.RegisterAttached("NotInRoleBehaviour", typeof(NotInRoleBehaviourEnum), typeof(SecurityManager),
                new PropertyMetadata(OnDemandRoleChanged));

        #region Property Getter/Setter

        public static string GetDemandRole(DependencyObject depObj)
        {
            return (string) depObj.GetValue(DemandRoleProperty);
        }

        public static void SetDemandRole(DependencyObject depObj, string roleName)
        {
            depObj.SetValue(DemandRoleProperty, roleName);
        }

        public static NotInRoleBehaviourEnum GetNotInRoleBehaviour(DependencyObject depObj)
        {
            return (NotInRoleBehaviourEnum)depObj.GetValue(NotInRoleBehaviourProperty);
        }

        public static void SetNotInRoleBehaviour(DependencyObject depObj, NotInRoleBehaviourEnum NotInRoleBehaviour)
        {
            depObj.SetValue(NotInRoleBehaviourProperty, NotInRoleBehaviour);
        }

        #endregion Property Getter/Setter


        private static void OnDemandRoleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var elm = d as FrameworkElement;
            if (elm != null)
            {
                if (e.NewValue != null && e.OldValue == null)
                {
                    ApplySecurityConstraints(elm);
                }
                else if (e.NewValue == null && e.OldValue != null)
                {
                }
            }
        }

        private static void ApplySecurityConstraints(FrameworkElement elm)
        {
            var demandRole = GetDemandRole(elm);
            var notInRoleBehaviour = GetNotInRoleBehaviour(elm);
            var isInRole = false;

            if (!string.IsNullOrEmpty(demandRole))
            {
                isInRole = Thread.CurrentPrincipal.IsInRole(demandRole);
                //isInRole = GateKeeper.CurrentPrincipal.IsInRole(demandRole);
            }

            if (!isInRole)
            {
                switch (notInRoleBehaviour)
                {
                    case NotInRoleBehaviourEnum.Disable:
                        elm.IsEnabled = false;
                        break;
                    case NotInRoleBehaviourEnum.Hide:
                        elm.Visibility = Visibility.Hidden;
                        break;
                    case NotInRoleBehaviourEnum.Collapse:
                        elm.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

    }
}