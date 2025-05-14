using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Collections.ObjectModel;

namespace QuanLyNhaSach.Messages;

public class SearchCompletedMessage<T>(ObservableCollection<T> results) : ValueChangedMessage<ObservableCollection<T>>(results) { }