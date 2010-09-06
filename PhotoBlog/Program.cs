using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FLocal.Client.Common;
using FLocal.Client.Common.actions;
using NConsoler;

namespace FLocal.Client.PhotoBlog {
	class Program {
		static void Main(string[] args) {
			Consolery.Run(typeof(Program), args);
		}

		[Action]
		public static void UploadPhoto(string filePath, string description) {

			try {

				using(RemoteSession session = new RemoteSession(Config.instance)) {

					UploadAction uploadAction = new UploadAction(filePath);
					int uploadId = uploadAction.Process(session);

					ReplyAction replyAction = new ReplyAction(
						Config.instance.mainPostId,
						1,
						description,
						String.Format("{0}\r\n\r\n[uploadImage]{1}[/uploadImage]", description, uploadId)
					);
					int postId = replyAction.Process(session);

					Console.WriteLine("Created post #" + postId);
				}
			} catch(Exception e) {
				Console.WriteLine(e.ToString());
			}
		}
	}
}
