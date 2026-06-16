namespace MeetSpot.Core.Interfaces;

public interface IBookingNotificationService
{
    Task NotifyBookingCreated(int roomId, string roomName, DateTime startTime, DateTime endTime);
    Task NotifyBookingCancelled(int roomId, string roomName);
}