using System.Windows;

namespace WpfDialogSampleApp.Views
{
    /// <summary>
    /// 再利用可能なダイアログのベースクラス
    /// </summary>
    public abstract class DialogBase : Window
    {
        private bool _isLoaded;

        protected DialogBase()
        {
            // 共通のダイアログ設定
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            Background = System.Windows.Media.Brushes.Transparent;
            
            Loaded += OnDialogLoaded;
            Closed += OnDialogClosed;
        }

        private void OnDialogLoaded(object sender, RoutedEventArgs e)
        {
            if (_isLoaded) return;
            _isLoaded = true;

            // 親ウィンドウの位置変更とサイズ変更を監視
            if (Owner != null)
            {
                Owner.LocationChanged += OnOwnerLocationChanged;
                Owner.SizeChanged += OnOwnerSizeChanged;
            }

            OnDialogLoadedCore();
        }

        private void OnDialogClosed(object? sender, EventArgs e)
        {
            // イベントハンドラーを解除
            if (Owner != null)
            {
                Owner.LocationChanged -= OnOwnerLocationChanged;
                Owner.SizeChanged -= OnOwnerSizeChanged;
            }

            OnDialogClosedCore();
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
            if (Owner != null && IsLoaded)
            {
                Left = Owner.Left + (Owner.Width - ActualWidth) / 2;
                Top = Owner.Top + (Owner.Height - ActualHeight) / 2;
            }
        }

        /// <summary>
        /// ダイアログが読み込まれた時の追加処理（派生クラスでオーバーライド可能）
        /// </summary>
        protected virtual void OnDialogLoadedCore() { }

        /// <summary>
        /// ダイアログが閉じられた時の追加処理（派生クラスでオーバーライド可能）
        /// </summary>
        protected virtual void OnDialogClosedCore() { }

        /// <summary>
        /// ダイアログを閉じる（結果付き）
        /// </summary>
        /// <param name="result">ダイアログの結果</param>
        public void CloseDialog(bool? result = null)
        {
            // モーダルダイアログの場合のみDialogResultを設定
            try
            {
                if (result.HasValue)
                {
                    DialogResult = result;
                }
            }
            catch (InvalidOperationException)
            {
                // 非モーダルダイアログの場合はDialogResultを設定できないため無視
            }
            
            Close();
        }
    }
}
