using CommunityToolkit.Mvvm.Messaging.Messages;

namespace QuanLyDaiLy.Messages;

public class SelectedDateMessage(int month, int year) : ValueChangedMessage<(int Month, int Year)>((month, year)) { }