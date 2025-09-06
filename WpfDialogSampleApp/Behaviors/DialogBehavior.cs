using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace WpfDialogSampleApp.Behaviors
{
    /// <summary>
    /// ダイアログの振る舞いを実装するXAMLビヘイビア
    /// </summary>
    public class DialogBehavior : Behavior<Window>
    {
        private bool _isLoaded;

        /// <summary>
        /// ダイアログが閉じられているかどうか
        /// </summary>
        public bool IsClosed { get; private set; }

        /// <summary>
        /// ビヘイビアがアタッチされた時に呼び出される
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            
            if (AssociatedObject != null)
            {
                // イベントハンドラーを追加
                AssociatedObject.Loaded += OnDialogLoaded;
                AssociatedObject.Closed += OnDialogClosed;
                
                // ダイアログ設定を適用
                ApplyDialogSettings();
            }
        }

        /// <summary>
        /// ビヘイビアがデタッチされた時に呼び出される
        /// </summary>
        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.Loaded -= OnDialogLoaded;
                AssociatedObject.Closed -= OnDialogClosed;

                // 親ウィンドウのイベントも解除
                if (AssociatedObject.Owner != null)
                {
                    AssociatedObject.Owner.LocationChanged -= OnOwnerLocationChanged;
                    AssociatedObject.Owner.SizeChanged -= OnOwnerSizeChanged;
                }
            }
            
            base.OnDetaching();
        }

        /// <summary>
        /// ダイアログを閉じる
        /// </summary>
        /// <param name="result">ダイアログの結果</param>
        public void CloseDialog(bool? result = null)
        {
            if (AssociatedObject == null) return;

            // モーダルダイアログの場合のみDialogResultを設定
            try
            {
                if (result.HasValue)
                {
                    AssociatedObject.DialogResult = result;
                }
            }
            catch (InvalidOperationException)
            {
                // 非モーダルダイアログの場合はDialogResultを設定できないため無視
            }
            
            AssociatedObject.Close();
        }

        private void ApplyDialogSettings()
        {
            if (AssociatedObject == null) return;

            // 共通のダイアログ設定
            AssociatedObject.ResizeMode = ResizeMode.NoResize;
            AssociatedObject.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            AssociatedObject.WindowStyle = WindowStyle.None;
            AssociatedObject.AllowsTransparency = true;
            AssociatedObject.Background = System.Windows.Media.Brushes.Transparent;
        }

        private void OnDialogLoaded(object sender, RoutedEventArgs e)
        {
            if (_isLoaded || AssociatedObject == null) return;
            _isLoaded = true;

            // 親ウィンドウの位置変更とサイズ変更を監視
            if (AssociatedObject.Owner != null)
            {
                AssociatedObject.Owner.LocationChanged += OnOwnerLocationChanged;
                AssociatedObject.Owner.SizeChanged += OnOwnerSizeChanged;
            }
        }

        private void OnDialogClosed(object? sender, EventArgs e)
        {
            IsClosed = true;
            
            // イベントハンドラーを解除
            if (AssociatedObject?.Owner != null)
            {
                AssociatedObject.Owner.LocationChanged -= OnOwnerLocationChanged;
                AssociatedObject.Owner.SizeChanged -= OnOwnerSizeChanged;
            }
        }

        private void OnOwnerLocationChanged(object? sender, EventArgs e)
        {
            // 親ウィンドウが移動したときにダイアログを中央に再配置
            CenterToOwner();
        }

        private void OnOwnerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // 親ウィンドウのサイズが変更されたときにダイアログを中央に再配置
            CenterToOwner();
        }

        private void CenterToOwner()
        {
            if (AssociatedObject?.Owner != null && AssociatedObject.IsLoaded)
            {
                AssociatedObject.Left = AssociatedObject.Owner.Left + (AssociatedObject.Owner.Width - AssociatedObject.ActualWidth) / 2;
                AssociatedObject.Top = AssociatedObject.Owner.Top + (AssociatedObject.Owner.Height - AssociatedObject.ActualHeight) / 2;
            }
        }
    }
}
