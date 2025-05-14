using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyNhaSach.Services
{
    public interface INavigationService
    {
        void NavigateTo<T>() where T : class;
        object CurrentViewModel { get; }
        event Action CurrentViewModelChanged;
    }

    public class NavigationService : INavigationService
    {
        private readonly Dictionary<Type, Type> _viewModelToPageMap = [];
        private readonly IServiceProvider _serviceProvider;
        private object _currentViewModel = null!;

        public object CurrentViewModel
        {
            get => _currentViewModel;
            private set
            {
                _currentViewModel = value;
                CurrentViewModelChanged?.Invoke();
            }
        }

        public event Action? CurrentViewModelChanged;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            RegisterViewModels();
        }

        private void RegisterViewModels()
        {
            // Register your view model to page mappings
            //_viewModelToPageMap.Add(typeof(ViewModels.DashboardViewModels.DashboardPageViewModel), typeof(Views.DashboardViews.DashboardPage));
            //_viewModelToPageMap.Add(typeof(ViewModels.LoaiDaiLyViewModels.LoaiDaiLyPageViewModel), typeof(Views.LoaiDaiLyViews.LoaiDaiLyPage));
            //_viewModelToPageMap.Add(typeof(ViewModels.PhieuXuatViewModels.PhieuXuatPageViewModel), typeof(Views.PhieuXuatViews.PhieuXuatPage));
            //_viewModelToPageMap.Add(typeof(ViewModels.DonViTinhViewModels.DonViTinhPageViewModel), typeof(Views.DonViTinhViews.DonViTinhPage));
            //_viewModelToPageMap.Add(typeof(ViewModels.ThamSoViewModels.ThamSoPageViewModel), typeof(Views.ThamSoViews.ThamSoPage));
            //_viewModelToPageMap.Add(typeof(ViewModels.MatHangViewModels.MatHangPageViewModel), typeof(Views.MatHangViews.MatHangPage));
            //_viewModelToPageMap.Add(typeof(ViewModels.QuanViewModels.QuanPageViewModel), typeof(Views.QuanViews.QuanPage));
            //_viewModelToPageMap.Add(typeof(ViewModels.PhieuThuViewModels.PhieuThuPageViewModel), typeof(Views.PhieuThuViews.PhieuThuPage));
        }

        public void NavigateTo<T>() where T : class
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("Service provider is not initialized.");

            if (_serviceProvider.GetService(typeof(T)) is not T viewModel)
                throw new InvalidOperationException($"Service of type {typeof(T)} is not registered.");

            CurrentViewModel = viewModel;
        }
    }
}
