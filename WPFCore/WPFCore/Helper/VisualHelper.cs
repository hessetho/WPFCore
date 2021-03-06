﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace WPFCore.Helper
{
    public static class VisualHelper
    {
        /// <summary>
        ///     Traverse the visual tree of an <c>element</c> to find a child of type <c>type</c>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Visual GetDescendantByType(Visual element, Type type)
        {
            if (element == null) return null;
            //if (element.GetType() == type) return element;

            Visual foundElement = null;
            if (element is FrameworkElement)
                (element as FrameworkElement).ApplyTemplate();

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var visual = VisualTreeHelper.GetChild(element, i) as Visual;
                if (visual.GetType() == type) return visual;

                foundElement = GetDescendantByType(visual, type);
                if (foundElement != null)
                    break;
            }
            return foundElement;
        }

        /// <summary>
        ///     Traverse the visual tree of an <c>element</c> to find a child of type <c>&gt;T%lt;</c>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static T GetDescendant<T>(Visual element) where T : Visual
        {
            var type = typeof (T);
            if (element == null) return default(T);

            var foundElement = default(T);
            var frameworkElement = element as FrameworkElement;
            if (frameworkElement != null)
                frameworkElement.ApplyTemplate();

            if (element.GetType() == type) return (T) element;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var visual = VisualTreeHelper.GetChild(element, i) as Visual;
                if (visual != null && visual.GetType() == type) return (T) visual;

                foundElement = GetDescendant<T>(visual);
                if (foundElement != null)
                    break;
            }

            return foundElement;
        }

        /// <summary>
        ///     Traverse the visual tree of an <c>element</c> to find a child of name
        ///     <param name="name" />
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name">Name of the child element</param>
        /// <returns></returns>
        public static FrameworkElement GetDescendantByName(DependencyObject element, string name)
        {
            if (element == null) return null;

            FrameworkElement foundElement = null;
            if (element is FrameworkElement)
                (element as FrameworkElement).ApplyTemplate();

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                if (child is FrameworkElement)
                    if (((FrameworkElement) child).Name == name) return (FrameworkElement) child;

                foundElement = GetDescendantByName(child, name);
                if (foundElement != null)
                    break;
            }
            return foundElement;
        }

        public static Visual GetParentByType(Visual element, Type type)
        {
            if (element == null) return null;

            if (element is FrameworkElement)
                (element as FrameworkElement).ApplyTemplate();

            var parent = VisualTreeHelper.GetParent(element) as Visual;
            if (parent == null || parent.GetType() == type)
                return parent;

            return GetParentByType(parent, type);
        }

        public static Visual GetParentByTypeName(Visual element, string typeName)
        {
            if (element == null) return null;

            if (element is FrameworkElement)
                (element as FrameworkElement).ApplyTemplate();

            var parent = VisualTreeHelper.GetParent(element) as Visual;
            if (parent == null || parent.GetType().Name == typeName)
                return parent;

            return GetParentByTypeName(parent, typeName);
        }

        public static void ClimbTheTree(Visual element)
        {
            if (element == null) return;
            //Debug.WriteLine(element.GetType().Name);

            if (element is FrameworkElement)
                (element as FrameworkElement).ApplyTemplate();

            var parent = VisualTreeHelper.GetParent(element) as Visual;
            if (parent != null) ClimbTheTree(parent);
        }

        public static T FindParent<T>(FrameworkElement element) where T : FrameworkElement
        {
            var parent = LogicalTreeHelper.GetParent(element) as FrameworkElement;

            while (parent != null)
            {
                var correctlyTyped = parent as T;
                if (correctlyTyped != null)
                    return correctlyTyped;
                return FindParent<T>(parent);
            }

            return null;
        }

        public static Rect GetRectOfObject(FrameworkElement element)
        {
            var rectangleBounds = new Rect();
            rectangleBounds = element.RenderTransform.TransformBounds(new Rect(element.RenderSize));
            return rectangleBounds;
        }

        public static List<Visual> EnumVisualTree(Visual vElementRoot)
        {
            var result = new List<Visual>();

            for (int intCounter = 0; intCounter < VisualTreeHelper.GetChildrenCount(vElementRoot); intCounter++)
            {
                //Get the control
                Visual child = (Visual)VisualTreeHelper.GetChild(vElementRoot, intCounter);
                //Add it to the list
                result.Add(child);

                //Process all children
                result.AddRange(EnumVisualTree(child));
            }

            return result;
        }
    }
}