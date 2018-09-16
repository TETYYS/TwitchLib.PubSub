﻿using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TwitchLib.PubSub.Models.Responses.Messages
{
    /// <inheritdoc />
    /// <summary>Class representing a whisper received via PubSub.</summary>
    public class Whisper : MessageData
    {
        /// <summary>Type of MessageData</summary>
        public string Type { get; protected set; }
        /// <summary>Enum of the Message type</summary>
        public Enums.WhisperType TypeEnum { get; protected set; }
        /// <summary>Data identifier in MessageData</summary>
        public string Data { get; protected set; }
        /// <summary>Object that houses the data accompanying the type.</summary>
        public DataObjWhisperReceived DataObjectWhisperReceived { get; protected set; }
        /// <summary>Object that houses the data accompanying the type.</summary>
        public DataObjThread DataObjectThread { get; protected set; }

        /// <summary>Whisper object constructor.</summary>
        public Whisper(string jsonStr)
        {
            var json = JObject.Parse(jsonStr);
            Type = json.SelectToken("type").ToString();
            Data = json.SelectToken("data").ToString();
            switch (Type)
            {
                case "whisper_received":
                    TypeEnum = Enums.WhisperType.WhisperReceived;
                    DataObjectWhisperReceived = new DataObjWhisperReceived(json.SelectToken("data_object"));
                    break;
                case "thread":
                    TypeEnum = Enums.WhisperType.Thread;
                    DataObjectThread = new DataObjThread(json.SelectToken("data_object"));
                    break;
                default:
                    TypeEnum = Enums.WhisperType.Unknown;
                    break;
            }
        }

        public class DataObjThread
        {
            public string Id { get; protected set; }
            public long LastRead { get; protected set; }
            public bool Archived { get; protected set; }
            public bool Muted { get; protected set; }
            public SpamInfoObj SpamInfo { get; protected set; }

            public DataObjThread(JToken json)
            {
                Id = json.SelectToken("id").ToString();
                LastRead = long.Parse(json.SelectToken("last_read").ToString());
                Archived = bool.Parse(json.SelectToken("archived").ToString());
                Muted = bool.Parse(json.SelectToken("muted").ToString());
                SpamInfo = new SpamInfoObj(json.SelectToken("spam_info"));
            }

            public class SpamInfoObj
            {
                public string Likelihood { get; protected set; }
                public long LastMarkedNotSpam { get; protected set; }

                public SpamInfoObj(JToken json)
                {
                    Likelihood = json.SelectToken("likelihood").ToString();
                    LastMarkedNotSpam = long.Parse(json.SelectToken("last_marked_not_spam").ToString());
                }
            }

        }

        /// <summary>Class representing the data in the MessageData object.</summary>
        public class DataObjWhisperReceived
        {
            /// <summary>DataObject identifier</summary>
            public string Id { get; protected set; }
            /// <summary>Twitch assigned thread id</summary>
            public string ThreadId { get; protected set; }
            /// <summary>Body of data received from Twitch</summary>
            public string Body { get; protected set; }
            /// <summary>Timestamp generated by Twitc</summary>
            public long SentTs { get; protected set; }
            /// <summary>Id of user that sent whisper.</summary>
            public string FromId { get; protected set; }
            /// <summary>Tags object housing associated tags.</summary>
            public TagsObj Tags { get; protected set; }
            /// <summary>Receipient object housing various properties about user who received whisper.</summary>
            public RecipientObj Recipient { get; protected set; }
            /// <summary>Uniquely generated string used to identify response from request.</summary>
            public string Nonce { get; protected set; }

            /// <summary>DataObj constructor.</summary>
            public DataObjWhisperReceived(JToken json)
            {
                Id = json.SelectToken("id").ToString();
                ThreadId = json.SelectToken("thread_id")?.ToString();
                Body = json.SelectToken("body")?.ToString();
                SentTs = long.Parse(json.SelectToken("sent_ts").ToString());
                FromId = json.SelectToken("from_id").ToString();
                Tags = new TagsObj(json.SelectToken("tags"));
                Recipient = new RecipientObj(json.SelectToken("recipient"));
                Nonce = json.SelectToken("nonce")?.ToString();
            }

            /// <summary>Class representing the tags associated with the whisper.</summary>
            public class TagsObj
            {
                /// <summary>Login value associated.</summary>
                public string Login { get; protected set; }
                /// <summary>Display name found in chat.</summary>
                public string DisplayName { get; protected set; }
                /// <summary>Color of whispers</summary>
                public string Color { get; protected set; }
                /// <summary>User type of whisperer</summary>
                public string UserType { get; protected set; }
                /// <summary>List of emotes found in whisper</summary>
                public readonly List<EmoteObj> Emotes = new List<EmoteObj>();
                /// <summary>All badges associated with the whisperer</summary>
                public readonly List<Badge> Badges = new List<Badge>();

                /// <summary></summary>
                public TagsObj(JToken json)
                {
                    Login = json.SelectToken("login")?.ToString();
                    DisplayName = json.SelectToken("login")?.ToString();
                    Color = json.SelectToken("color")?.ToString();
                    UserType = json.SelectToken("user_type")?.ToString();
                    foreach (var emote in json.SelectToken("emotes"))
                        Emotes.Add(new EmoteObj(emote));
                    foreach (var badge in json.SelectToken("badges"))
                        Badges.Add(new Badge(badge));
                }

                /// <summary>Class representing a single emote found in a whisper</summary>
                public class EmoteObj
                {
                    /// <summary>Emote ID</summary>
                    public int Id { get; protected set; }
                    /// <summary>Starting character of emote</summary>
                    public int Start { get; protected set; }
                    /// <summary>Ending character of emote</summary>
                    public int End { get; protected set; }

                    /// <summary>EmoteObj construcotr.</summary>
                    public EmoteObj(JToken json)
                    {
                        Id = int.Parse(json.SelectToken("id").ToString());
                        Start = int.Parse(json.SelectToken("start").ToString());
                        End = int.Parse(json.SelectToken("end").ToString());
                    }
                }
            }

            /// <summary>Class representing the recipient of the whisper.</summary>
            public class RecipientObj
            {
                /// <summary>Receiver id</summary>
                public string Id { get; protected set; }
                /// <summary>Receiver username</summary>
                public string Username { get; protected set; }
                /// <summary>Receiver display name.</summary>
                public string DisplayName { get; protected set; }
                /// <summary>Receiver color.</summary>
                public string Color { get; protected set; }
                /// <summary>User type of receiver.</summary>
                public string UserType { get; protected set; }
                /// <summary>List of badges that the receiver has.</summary>
                public List<Badge> Badges { get; protected set; } = new List<Badge>();

                /// <summary>RecipientObj constructor.</summary>
                public RecipientObj(JToken json)
                {
                    Id = json.SelectToken("id").ToString();
                    Username = json.SelectToken("username")?.ToString();
                    DisplayName = json.SelectToken("display_name")?.ToString();
                    Color = json.SelectToken("color")?.ToString();
                    UserType = json.SelectToken("user_type")?.ToString();
                    foreach (var badge in json.SelectToken("badges"))
                        Badges.Add(new Badge(badge));
                }
            }

            /// <summary>Class representing a single badge.</summary>
            public class Badge
            {
                /// <summary>Id of the badge.</summary>
                public string Id { get; protected set; }
                /// <summary>Version of the badge.</summary>
                public string Version { get; protected set; }

                /// <summary></summary>
                public Badge(JToken json)
                {
                    Id = json.SelectToken("id")?.ToString();
                    Version = json.SelectToken("version")?.ToString();
                }
            }
        }
    }
}