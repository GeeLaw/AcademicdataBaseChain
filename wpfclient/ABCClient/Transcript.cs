using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCClient
{
    public class TranscriptCore : INotifyPropertyChanged
    {
        public TranscriptCore() { }

        [JsonIgnore]
        public IReadOnlyList<Entity> Entities { get { return Entity.Entities; } }

        Entity student;
        [JsonIgnore]
        public Entity Student
        {
            get { return student; }
            set
            {
                if (!object.ReferenceEquals(student, value))
                {
                    student = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Student"));
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("StudentId"));
                }
            }
        }
        [JsonProperty("student")]
        public string StudentId
        {
            get { return Student?.AccountAddress; }
            set
            {
                Student = Entity.Entities.FirstOrDefault(x => x.AccountAddress == value);
            }
        }
        string courseId;
        [JsonProperty("course")]
        public string CourseId
        {
            get
            {
                return courseId;
            }
            set
            {
                if (value != courseId)
                {
                    courseId = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CourseId"));
                }
            }
        }
        ulong year;
        [JsonProperty("year")]
        public ulong Year
        {
            get
            {
                return year;
            }
            set
            {
                if (value != year)
                {
                    year = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Year"));
                }
            }
        }
        ulong term;
        [JsonProperty("term")]
        public ulong Term
        {
            get { return term; }
            set
            {
                if (value != term)
                {
                    term = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Term"));
                }
            }
        }
        ulong credit;
        [JsonProperty("credit")]
        public ulong Credit
        {
            get { return credit; }
            set
            {
                if (value != credit)
                {
                    credit = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Credit"));
                }
            }
        }
        ulong grade;
        [JsonProperty("grade")]
        public ulong Grade
        {
            get { return grade; }
            set
            {
                if (value != grade)
                {
                    grade = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Grade"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };
    }
}
