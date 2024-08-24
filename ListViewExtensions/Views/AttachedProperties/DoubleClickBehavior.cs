using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace ListViewExtensions.Views.AttachedProperties
{
	/// <summary>
	/// ListViewItemでのダブルクリックイベントをViewModelで受信するための添付ビヘイビア
	/// このクラスを使えば、コマンドでイベントを受信する方法とメソッドで直接イベントを受信する方法の2通りが取れます。
	/// コマンドで受信する場合は、Command添付プロパティに受信するコマンドを登録してください。
	/// メソッドで受信する場合は、MethodName添付プロパティにメソッド名、MethodTarget添付プロパティにそのメソッドがあるオブジェクトを設定してください。
	/// 呼び出されるメソッドは、MouseButtonEventArgs型の引数1個を持ったものか、もしくは引数が無いものです。前者のほうが優先度が高く、前者のパターンのメソッドが無ければ後者のパターンが呼び出されます。
	/// </summary>
	public static class DoubleClickBehavior
	{
		#region Command

		public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
			Regex.Replace(nameof(CommandProperty), "Property$", ""),	//末尾のPropertyを消す
			typeof(ICommand),
			typeof(DoubleClickBehavior),
			new FrameworkPropertyMetadata(null, (sender, e)=> {
				Control ctrl = sender as Control;

				if(ctrl != null) {
					ICommand oldCommand = (ICommand)e.OldValue;
					ICommand newCommand = (ICommand)e.NewValue;

					if((oldCommand != null) && (newCommand == null))	//購読を停止
						ctrl.MouseDoubleClick -= Control_MouseDoubleClick_ForCommand;
					if((oldCommand == null) && (newCommand != null))    //購読を開始
						ctrl.MouseDoubleClick += Control_MouseDoubleClick_ForCommand; ;
				}
			}));

		public static void SetCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(CommandProperty, value);
		}

		public static ICommand GetCommand(DependencyObject obj)
		{
			return (ICommand)obj.GetValue(CommandProperty);
		}

		private static void Control_MouseDoubleClick_ForCommand(object sender, MouseButtonEventArgs e)
		{
			ICommand com = GetCommand((Control)sender);

			if(com.CanExecute(e))
				com.Execute(e);
		}

		#endregion

		#region Method

		#region MethodName

		public static readonly DependencyProperty MethodNameProperty = DependencyProperty.RegisterAttached(
			Regex.Replace(nameof(MethodNameProperty), "Property$", ""),    //末尾のPropertyを消す
			typeof(string),
			typeof(DoubleClickBehavior),
			new FrameworkPropertyMetadata(null, (sender, e) => {
				Control ctrl = sender as Control;
				object target = ctrl != null ? GetMethodTarget(ctrl) : null;

				if(ctrl != null && target != null) {
					string oldName = (string)e.OldValue;
					string newName = (string)e.NewValue;

					MethodInfo[] oldMethods = string.IsNullOrEmpty(oldName) ?
						new MethodInfo[] { null, null } :
						new[] {
							target.GetType().GetMethod(oldName, new[] { typeof(MouseEventArgs) }),
							target.GetType().GetMethod(oldName, Type.EmptyTypes)
						};
					MethodInfo[] newMethods = string.IsNullOrEmpty(newName) ?
						new MethodInfo[] { null, null } :
						new[] {
							target.GetType().GetMethod(newName, new[] { typeof(MouseEventArgs) }),
							target.GetType().GetMethod(newName, Type.EmptyTypes)
						};

					if(oldMethods.Any(p => p != null) && newMethods.All(p => p == null))    //購読を停止
						ctrl.MouseDoubleClick -= Control_MouseDoubleClick_ForMethod;
					if(oldMethods.All(p => p == null) && newMethods.Any(p => p != null))    //購読を開始
						ctrl.MouseDoubleClick += Control_MouseDoubleClick_ForMethod; ;
				}
			}));

		public static string GetMethodName(DependencyObject obj)
		{
			return (string)obj.GetValue(MethodNameProperty);
		}

		public static void SetMethodName(DependencyObject obj, string value)
		{
			obj.SetValue(MethodNameProperty, value);
		}

		#endregion

		#region MethodTarget

		public static readonly DependencyProperty MethodTargetProperty = DependencyProperty.RegisterAttached(
			Regex.Replace(nameof(MethodTargetProperty), "Property$", ""),    //末尾のPropertyを消す
			typeof(object),
			typeof(DoubleClickBehavior),
			new FrameworkPropertyMetadata(null, (sender, e) => {
				Control ctrl = sender as Control;
				string methodname = ctrl != null ? GetMethodName(ctrl) : null;

				if(ctrl != null && !string.IsNullOrEmpty(methodname)) {
					object oldTarget = (object)e.OldValue;
					object newTarget = (object)e.NewValue;

					MethodInfo[] oldMethods = new[] {
						oldTarget?.GetType()?.GetMethod(methodname, new[] { typeof(MouseEventArgs) }),
						oldTarget?.GetType()?.GetMethod(methodname, Type.EmptyTypes)
					};
					MethodInfo[] newMethods = new[] {
						newTarget?.GetType()?.GetMethod(methodname, new[] { typeof(MouseEventArgs) }),
						newTarget?.GetType()?.GetMethod(methodname, Type.EmptyTypes)
					};

					if(oldMethods.Any(p => p != null) && newMethods.All(p => p == null))    //購読を停止
						ctrl.MouseDoubleClick -= Control_MouseDoubleClick_ForMethod;
					if(oldMethods.All(p => p == null) && newMethods.Any(p => p != null))    //購読を開始
						ctrl.MouseDoubleClick += Control_MouseDoubleClick_ForMethod; ;
				}
			}));

		public static object GetMethodTarget(DependencyObject obj)
		{
			return obj.GetValue(MethodTargetProperty);
		}

		public static void SetMethodTarget(DependencyObject obj, object value)
		{
			obj.SetValue(MethodTargetProperty, value);
		}

		#endregion

		private static void Control_MouseDoubleClick_ForMethod(object sender, MouseButtonEventArgs e)
		{
			Control ctrl = (Control)sender;

			object target = GetMethodTarget(ctrl);
			string methodname = GetMethodName(ctrl);

			MethodInfo[] methods = new[] {
				target.GetType().GetMethod(methodname, new[] { typeof(MouseEventArgs) }),
				target.GetType().GetMethod(methodname, Type.EmptyTypes)
			};

			if(methods[0] != null)
				methods[0].Invoke(target, new object[] { e });
			else if(methods[1] != null)
				methods[1].Invoke(target, new object[] { });
		}

		#endregion
	}
}
