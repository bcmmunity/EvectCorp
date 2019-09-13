using System.Collections.Generic;
using Evect.Models;
namespace Evect.Models
{
    public class User
    {
        public int UserId { get; set; }
        public long TelegramId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyAndPosition { get; set; }
        public string Utility { get; set; }
        public string Communication { get; set; }
        public string Phone { get; set; }
        public bool IsAuthed { get; set; }
        public bool IsAdminAuthorized { get; set; }
        
        public Actions CurrentAction { get; set; } = Actions.None;
        
        public int CurrentEventId { get; set; }
        public List<UserTag> UserTags { get; set; } = new List<UserTag>();
        public List<UserSearchingTag> SearchingUserTags { get; set; } = new List<UserSearchingTag>();
        public List<UserEvent> UserEvents { get; set; } = new List<UserEvent>();
        public int CurrentSurveyId { get; set; }
        public int CurrentQuestionId { get; set; }//��� ����� ��������� ���������� ����� ����������
        public Actions PreviousAction { get; set; }

        public List<ContactsBook> Contacts { get; set; }
        public string TelegramUserName{ get; set; }
        
    }
}