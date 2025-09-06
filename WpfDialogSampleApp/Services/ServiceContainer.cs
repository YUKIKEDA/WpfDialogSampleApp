using System;
using System.Collections.Generic;

namespace WpfDialogSampleApp.Services
{
    /// <summary>
    /// シンプルなDIコンテナー実装
    /// </summary>
    public class ServiceContainer
    {
        private readonly Dictionary<Type, Func<object>> _services = new();
        private readonly Dictionary<Type, object> _singletons = new();

        /// <summary>
        /// サービスを一時的なライフタイムで登録
        /// </summary>
        public void RegisterTransient<TInterface, TImplementation>()
            where TImplementation : class, TInterface, new()
        {
            _services[typeof(TInterface)] = () => new TImplementation();
        }

        /// <summary>
        /// サービスを一時的なライフタイムで登録（ファクトリー関数付き）
        /// </summary>
        public void RegisterTransient<TInterface>(Func<TInterface> factory)
        {
            _services[typeof(TInterface)] = () => factory()!;
        }

        /// <summary>
        /// サービスをシングルトンライフタイムで登録
        /// </summary>
        public void RegisterSingleton<TInterface, TImplementation>()
            where TImplementation : class, TInterface, new()
        {
            _services[typeof(TInterface)] = () =>
            {
                if (!_singletons.ContainsKey(typeof(TInterface)))
                {
                    _singletons[typeof(TInterface)] = new TImplementation();
                }
                return _singletons[typeof(TInterface)];
            };
        }

        /// <summary>
        /// サービスをシングルトンライフタイムで登録（ファクトリー関数付き）
        /// </summary>
        public void RegisterSingleton<TInterface, TImplementation>(Func<TImplementation> factory)
            where TImplementation : class, TInterface
        {
            _services[typeof(TInterface)] = () =>
            {
                if (!_singletons.ContainsKey(typeof(TInterface)))
                {
                    _singletons[typeof(TInterface)] = factory();
                }
                return _singletons[typeof(TInterface)];
            };
        }

        /// <summary>
        /// サービスをシングルトンライフタイムで登録（インスタンス付き）
        /// </summary>
        public void RegisterSingleton<TInterface>(TInterface instance)
            where TInterface : class
        {
            _singletons[typeof(TInterface)] = instance;
            _services[typeof(TInterface)] = () => _singletons[typeof(TInterface)];
        }

        /// <summary>
        /// サービスを取得
        /// </summary>
        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        /// <summary>
        /// サービスを取得
        /// </summary>
        public object GetService(Type serviceType)
        {
            if (_services.TryGetValue(serviceType, out var factory))
            {
                return factory();
            }

            throw new InvalidOperationException($"サービス {serviceType.Name} は登録されていません。");
        }

        /// <summary>
        /// サービスの取得を試行
        /// </summary>
        public T? TryGetService<T>() where T : class
        {
            try
            {
                return GetService<T>();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// サービスの取得を試行（型指定）
        /// </summary>
        public object? TryGetService(Type serviceType)
        {
            try
            {
                return GetService(serviceType);
            }
            catch
            {
                return null;
            }
        }
    }
}
