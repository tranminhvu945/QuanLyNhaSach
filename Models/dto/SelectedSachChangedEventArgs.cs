namespace QuanLyNhaSach.Models.dto
{
    public class SelectedSachChangedEventArgs : EventArgs
    {
        public Sach OldSach { get; }
        public Sach NewSach { get; }

        public SelectedSachChangedEventArgs(Sach oldSach, Sach newSach)
        {
            OldSach = oldSach;
            NewSach = newSach;
        }
    }
}