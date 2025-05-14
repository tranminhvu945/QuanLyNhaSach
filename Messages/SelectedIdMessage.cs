using CommunityToolkit.Mvvm.Messaging.Messages;

namespace QuanLyDaiLy.Messages;

public class SelectedIdMessage(int id) : ValueChangedMessage<int>(id) { }