using System;
using Assets.SimpleAndroidNotifications.Data;
using Assets.SimpleAndroidNotifications.Enums;
using Assets.SimpleAndroidNotifications.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.SimpleAndroidNotifications {
	
	public class HamsterNotifier : MonoBehaviour {

		public void Awake() {
			var callback = NotificationManager.GetNotificationCallback();

			if (callback != null) {
				Popup.Instance.ShowMessage("The app was started by clicking a notification, callback.Data = " + callback.Data);
			}
		}

		public void OnApplicationPause(bool pause) {
			if (!pause) {
				var callback = NotificationManager.GetNotificationCallback();

				if (callback != null) {
					Popup.Instance.ShowMessage("The app was resumed by clicking a notification, callback.Data = " + callback.Data);
				}
			}
		}

		public void Rate() { Application.OpenURL("http://u3d.as/A6d"); }
		public void OpenWiki() { Application.OpenURL("https://github.com/hippogamesunity/SimpleAndroidNotificationsPublic/wiki"); }

		public void CancelAll() { NotificationManager.CancelAll(); }

		public void ScheduleSimple(int seconds) {
			NotificationManager.Send(TimeSpan.FromSeconds(seconds), "Simple notification",
				"Please rate the asset on the Asset Store!", new Color(1f, 0.3f, 0.15f));
		}

		/// <summary>
		/// Note: as of API 19, all repeating alarms are inexact. If your application needs precise delivery times then it must use one-time exact alarms, rescheduling each time as described above.
		/// </summary>
		public void ScheduleNormal() {
			NotificationManager.SendWithAppIcon(TimeSpan.FromSeconds(5), "Notification", "Notification with app icon", new Color(0f, 0.6f, 1f), NotificationIcon.Message);
		}

		public void ScheduleBreakTimer(int seconds) {
			var notificationParams = new NotificationParams {
				Id = NotificationIdHandler.GetNotificationId(),
				Delay = TimeSpan.FromSeconds(seconds),
				//Delay = TimeSpan.FromSeconds(20),
				Title = "Hamster World",
				Message = "Break Over! Come back to class!",
				Sound = true,
				Vibrate = true,
				SmallIcon = NotificationIcon.Bell,
				SmallIconColor = new Color(0f, 0.5f, 0f),
				LargeIcon = "app_icon",
				ExecuteMode = NotificationExecuteMode.Inexact,
				Importance = NotificationImportance.Max,
				CallbackData = "notification created at " + DateTime.Now
			};
			NotificationManager.SendCustom(notificationParams);
		}
		public void ScheduleBuzzer(int minutes) {
			var notificationParams = new NotificationParams {
				Id = NotificationIdHandler.GetNotificationId(),
				Title = "Hamster World",
				Message = "How you doing?",
				Sound = false,
				Vibrate = true,
				SmallIcon = NotificationIcon.Heart,
				SmallIconColor = new Color(0.8f, 0f, 0.5f),
				LargeIcon = "app_icon",
				ExecuteMode = NotificationExecuteMode.Inexact,
				Importance = NotificationImportance.Max,
				Repeat = true,
				//RepeatInterval = TimeSpan.FromSeconds(30)
				RepeatInterval = TimeSpan.FromMinutes(minutes)
			};
			NotificationManager.SendCustom(notificationParams);
		}		

		public void ScheduleRepeated() {
			var notificationParams = new NotificationParams {
				Id = NotificationIdHandler.GetNotificationId(),
				Delay = TimeSpan.FromSeconds(5),
				Title = "Repeated notification",
				Message = "Please rate the asset on the Asset Store!",
				Ticker = "This is repeated message ticker!",
				Sound = true,
				Vibrate = true,
				Vibration = new[] {500, 500, 500, 500, 500, 500},
				Light = true,
				LightOnMs = 1000,
				LightOffMs = 1000,
				LightColor = Color.magenta,
				SmallIcon = NotificationIcon.Skull,
				SmallIconColor = new Color(0f, 0.5f, 0f),
				LargeIcon = "app_icon",
				ExecuteMode = NotificationExecuteMode.Inexact,
				Repeat = true,
				RepeatInterval = TimeSpan.FromSeconds(30) // Don't use short intervals as repeated notifications are always inexact
			};

			NotificationManager.SendCustom(notificationParams);
		}

		public void ScheduleMultiline() {
			var notificationParams = new NotificationParams {
				Id = NotificationIdHandler.GetNotificationId(),
				Delay = TimeSpan.FromSeconds(5),
				Title = "Multiline notification",
				Message = "Line#1\nLine#2\nLine#3\nLine#4",
				Ticker = "This is multiline message ticker!",
				Multiline = true
			};

			NotificationManager.SendCustom(notificationParams);
		}

		public void ScheduleGrouped() {
			var id = NotificationIdHandler.GetNotificationId();
			var notificationParams = new NotificationParams {
				Id = id,
				GroupName = "Group",
				GroupSummary = "{0} new messages",
				Delay = TimeSpan.FromSeconds(5),
				Title = "Grouped notification",
				Message = "Message " + id,
				Ticker = "Please rate the asset on the Asset Store!",
				LargeIcon = "app_icon"
			};

			NotificationManager.SendCustom(notificationParams);
		}

		public void ScheduleCustom() {
			var notificationParams = new NotificationParams {
				Id = NotificationIdHandler.GetNotificationId(),
				Delay = TimeSpan.FromSeconds(5),
				Title = "Notification with callback",
				Message = "Open app and check the checkbox!",
				Ticker = "Notification with callback",
				Sound = true,
				//CustomSound = "ding", // AAR\res\raw\ding.wav. Please refer to plugin manual to learn how to add custom sounds.
				Vibrate = true,
				Vibration = new[] {500, 500, 500, 500, 500, 500},
				Light = true,
				LightOnMs = 1000,
				LightOffMs = 1000,
				LightColor = Color.red,
				SmallIcon = NotificationIcon.Sync,
				SmallIconColor = new Color(0f, 0.5f, 0f),
				LargeIcon = "app_icon",
				ExecuteMode = NotificationExecuteMode.Inexact,
				Importance = NotificationImportance.Max,
				CallbackData = "notification created at " + DateTime.Now
			};

			NotificationManager.SendCustom(notificationParams);
		}

		public void ScheduleWithChannel() {
			var notificationParams = new NotificationParams {
				Id = NotificationIdHandler.GetNotificationId(),
				Delay = TimeSpan.FromSeconds(5),
				Title = "Notification with news channel",
				Message = "Check the channel in your app settings!",
				Ticker = "Notification with news channel",
				ChannelId = "com.company.app.news",
				ChannelName = "News"
			};

			NotificationManager.SendCustom(notificationParams);

			if (NotificationManager.GetNotificationChannelIds().Contains(notificationParams.ChannelId)) {
				Popup.Instance.ShowMessage("Channel created: " + notificationParams.ChannelId);
			} else {
				Popup.Instance.ShowMessage("Channel was not created: " + notificationParams.ChannelId);
			}
		}

		public void DeleteAllNotificationChannels() {
			var channelIds = NotificationManager.GetNotificationChannelIds();

			foreach (var channelId in channelIds) { NotificationManager.DeleteNotificationChannel(channelId); }

			if (NotificationManager.GetNotificationChannelIds().Count == 0) {
				Popup.Instance.ShowMessage("All channels deleted: " + string.Join(", ", channelIds.ToArray()));
			} else {
				Popup.Instance.ShowMessage("Channels were not deleted: " + string.Join(", ", NotificationManager.GetNotificationChannelIds().ToArray()));
			}
		}
	}
}