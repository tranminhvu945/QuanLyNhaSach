using CommunityToolkit.Mvvm.Messaging.Messages;

namespace QuanLyNhaSach.Messages;

public class SelectedIdMessage(int id) : ValueChangedMessage<int>(id) { }