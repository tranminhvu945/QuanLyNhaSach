using CommunityToolkit.Mvvm.Messaging.Messages;

namespace QuanLyNhaSach.Messages;

public class SelectedDateMessage(int month, int year) : ValueChangedMessage<(int Month, int Year)>((month, year)) { }