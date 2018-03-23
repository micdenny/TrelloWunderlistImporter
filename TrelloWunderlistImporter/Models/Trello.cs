using System;

#pragma warning disable IDE1006 // Naming Styles

namespace TrelloWunderlistImporter.Models.Trello
{
    public class Board
    {
        public string id { get; set; }
        public string name { get; set; }
        public List[] lists { get; set; }
    }

    public class List
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool closed { get; set; }
        public string idBoard { get; set; }
        public float pos { get; set; }
    }

    public class Card
    {
        public string id { get; set; }
        public Badges badges { get; set; }
        public object[] checkItemStates { get; set; }
        public bool closed { get; set; }
        public bool dueComplete { get; set; }
        public DateTime dateLastActivity { get; set; }
        public string desc { get; set; }
        public object due { get; set; }
        public object email { get; set; }
        public string idBoard { get; set; }
        public object[] idChecklists { get; set; }
        public string idList { get; set; }
        public object[] idMembers { get; set; }
        public object[] idMembersVoted { get; set; }
        public int idShort { get; set; }
        public object idAttachmentCover { get; set; }
        public object[] labels { get; set; }
        public object[] idLabels { get; set; }
        public bool manualCoverAttachment { get; set; }
        public string name { get; set; }
        public int pos { get; set; }
        public string shortLink { get; set; }
        public string shortUrl { get; set; }
        public bool subscribed { get; set; }
        public object[] stickers { get; set; }
        public string url { get; set; }
    }

    public class Badges
    {
        public int votes { get; set; }
        public bool viewingMemberVoted { get; set; }
        public bool subscribed { get; set; }
        public string fogbugz { get; set; }
        public int checkItems { get; set; }
        public int checkItemsChecked { get; set; }
        public int comments { get; set; }
        public int attachments { get; set; }
        public bool description { get; set; }
        public object due { get; set; }
        public bool dueComplete { get; set; }
    }

    public class Checklist
    {
        public string id { get; set; }
        public string name { get; set; }
        public string idBoard { get; set; }
        public string idCard { get; set; }
        public int pos { get; set; }
        public object[] checkItems { get; set; }
    }
    
    public class ChecklistItem
    {
        public string state { get; set; }
        public string idChecklist { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public int pos { get; set; }
    }
}

#pragma warning restore IDE1006 // Naming Styles