using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TrelloWunderlistImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new Application();
            app.Run().Wait();
        }
    }

    public class Application
    {
        private const string WUNDERLIST_BACKUP_FILENAME = "wunderlist-backup.json";
        private const string TRELLO_KEY = "";
        private const string TRELLO_TOKEN = "";
        private const string TRELLO_BOARD_SHORTID = "";

        private readonly HttpClient _client;

        public Application()
        {
            _client = new HttpClient();
        }

        public async Task Run()
        {
            string wunderlistFileContent = File.ReadAllText(WUNDERLIST_BACKUP_FILENAME);
            var wunderlist = JsonConvert.DeserializeObject<Models.Wunderlist.WunderlistBackup>(wunderlistFileContent);

            var board = await GetBoard(TRELLO_BOARD_SHORTID);

            foreach (var wList in wunderlist.data.lists)
            {
                var taskPositions = wunderlist.data.task_positions.FirstOrDefault(x => x.list_id == wList.id).values.Select((x, index) => new { Position = index + 1, TaskId = x }).ToList();

                // todo list
                var todoList = await AddList(board.id, wList.title + " - TODO");
                foreach (var wTask in wunderlist.data.tasks.Where(x => x.list_id == wList.id && !x.completed))
                {
                    // card
                    var pos = taskPositions.FirstOrDefault(x => x.TaskId == wTask.id)?.Position.ToString() ?? "bottom";
                    var notes = wunderlist.data.notes.FirstOrDefault(x => x.task_id == wTask.id);
                    var card = await AddCard(todoList.id, wTask.title, notes?.content, pos);

                    // card checklist
                    var wSubTasks = wunderlist.data.subtasks.Where(x => x.task_id == wTask.id).ToList();
                    if (wSubTasks.Count > 0)
                    {
                        var checklistItemPositions = wunderlist.data.subtask_positions.FirstOrDefault(x => x.task_id == wTask.id).values.Select((x, index) => new { Position = index + 1, CheckListItemId = x }).ToList();
                        var checklist = await AddChecklist(card.id, "Tasks");
                        foreach (var wSubTask in wSubTasks)
                        {
                            var itemPos = checklistItemPositions.FirstOrDefault(x => x.CheckListItemId == wSubTask.id)?.Position.ToString() ?? "bottom";
                            var item = AddChecklistItem(checklist.id, wSubTask.title, itemPos, wSubTask.completed);
                        }
                    }
                }

                // done list
                var doneList = await AddList(board.id, wList.title + " - DONE");
                foreach (var wTask in wunderlist.data.tasks.Where(x => x.list_id == wList.id && x.completed))
                {
                    // card
                    var pos = taskPositions.FirstOrDefault(x => x.TaskId == wTask.id)?.Position.ToString() ?? "bottom";
                    var notes = wunderlist.data.notes.FirstOrDefault(x => x.task_id == wTask.id);
                    var card = await AddCard(doneList.id, wTask.title, notes?.content, pos);

                    // card checklist
                    var wSubTasks = wunderlist.data.subtasks.Where(x => x.task_id == wTask.id).ToList();
                    if (wSubTasks.Count > 0)
                    {
                        var checklistItemPositions = wunderlist.data.subtask_positions.FirstOrDefault(x => x.task_id == wTask.id).values.Select((x, index) => new { Position = index + 1, CheckListItemId = x }).ToList();
                        var checklist = await AddChecklist(card.id, "Tasks");
                        foreach (var wSubTask in wSubTasks)
                        {
                            var itemPos = checklistItemPositions.FirstOrDefault(x => x.CheckListItemId == wSubTask.id)?.Position.ToString() ?? "bottom";
                            var item = AddChecklistItem(checklist.id, wSubTask.title, itemPos, wSubTask.completed);
                        }
                    }
                }
            }
        }
        
        public async Task<Models.Trello.Board> GetBoard(string id)
        {
            var resp = await _client.GetAsync($@"https://api.trello.com/1/boards/{id}?fields=id,name&lists=open&list_fields=id,name,pos&key={TRELLO_KEY}&token={TRELLO_TOKEN}");
            var data = await resp.Content.ReadAsStringAsync();
            Console.WriteLine(data);
            var board = JsonConvert.DeserializeObject<Models.Trello.Board>(data);
            return board;
        }

        public async Task<Models.Trello.List> AddList(string idBoard, string name)
        {
            var resp = await _client.PostAsync($@"https://api.trello.com/1/lists?name={Uri.EscapeDataString(name)}&idBoard={idBoard}&key={TRELLO_KEY}&token={TRELLO_TOKEN}", null);
            var data = await resp.Content.ReadAsStringAsync();
            Console.WriteLine(data);
            var list = JsonConvert.DeserializeObject<Models.Trello.List>(data);
            return list;
        }

        public async Task<Models.Trello.Card> AddCard(string idList, string name, string desc, string pos = "bottom")
        {
            if (string.IsNullOrWhiteSpace(desc))
                desc = string.Empty;

            var resp = await _client.PostAsync($@"https://api.trello.com/1/cards?idList={idList}&name={Uri.EscapeDataString(name)}&pos={pos}&desc={Uri.EscapeDataString(desc)}&key={TRELLO_KEY}&token={TRELLO_TOKEN}", null);
            var data = await resp.Content.ReadAsStringAsync();
            Console.WriteLine(data);
            var card = JsonConvert.DeserializeObject<Models.Trello.Card>(data);
            return card;
        }

        public async Task<Models.Trello.Checklist> AddChecklist(string idCard, string name)
        {
            var resp = await _client.PostAsync($@"https://api.trello.com/1/checklists?idCard={idCard}&name={Uri.EscapeDataString(name)}&key={TRELLO_KEY}&token={TRELLO_TOKEN}", null);
            var data = await resp.Content.ReadAsStringAsync();
            Console.WriteLine(data);
            var checklist = JsonConvert.DeserializeObject<Models.Trello.Checklist>(data);
            return checklist;
        }

        public async Task<Models.Trello.ChecklistItem> AddChecklistItem(string idChecklist, string name, string pos = "bottom", bool isChecked = false)
        {
            var resp = await _client.PostAsync($@"https://api.trello.com/1/checklists/{idChecklist}/checkItems?name={Uri.EscapeDataString(name)}&pos={pos}&checked={isChecked.ToString().ToLower()}&key={TRELLO_KEY}&token={TRELLO_TOKEN}", null);
            var data = await resp.Content.ReadAsStringAsync();
            Console.WriteLine(data);
            var checklistItem = JsonConvert.DeserializeObject<Models.Trello.ChecklistItem>(data);
            return checklistItem;
        }
    }
}
