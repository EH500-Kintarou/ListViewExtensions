using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ListViewExtensions.Models;
using Livet;

namespace Sample.Models
{
	public class MainWindowModel : NotificationObject
	{
		#region Singleton

		static MainWindowModel? Instance = null;

		public static MainWindowModel GetInstance()
		{
			Instance ??= new MainWindowModel();
			return Instance;
		}

		#endregion

		Random random = new Random();
		readonly PersonModel[] array = [
			new PersonModel() { Name = "宮内 れんげ", Pronunciation = "みやうち れんげ"  , Birthday = new DateTime(2008, 12,  3), Age =  7, Height_cm = 139, },
			new PersonModel() { Name = "一条 蛍"    , Pronunciation = "いちじょう ほたる", Birthday = new DateTime(2004,  5, 28), Age = 11, Height_cm = 164, },
			new PersonModel() { Name = "越谷 夏海"  , Pronunciation = "こしがや なつみ"  , Birthday = new DateTime(2003,  1, 24), Age = 12, Height_cm = 155, },
			new PersonModel() { Name = "越谷 小毬"  , Pronunciation = "こしがや こまり"  , Birthday = new DateTime(2001,  9, 14), Age = 14, Height_cm = 140, },
			new PersonModel() { Name = "越谷 卓"    , Pronunciation = "こしがや すぐる"  , Birthday = new DateTime(2000,  4, 11), Age = 15, Height_cm = 171, },
		];

		public SortableObservableCollection<PersonModel> People { get; } = new SortableObservableCollection<PersonModel>();

		private MainWindowModel()
		{
			//AddLoop();
		}

		void AddLoop()
		{
			Task.Run(async () => {
				while(true) {
					await Task.Delay(TimeSpan.FromSeconds(1));
					
					Parallel.ForEach(Enumerable.Range(0, 200), p => {
						if(p % 2 == 0 && People.Count > 0) {
							lock(People.SyncRoot)
								People.RemoveAt(random.Next(People.Count));
						} else
							Add();
					});				
				}
			});
		}

		public void Add()
		{
			AddRange(1);
		}

		public void AddRange(int count)
		{
			People.AddRange(Enumerable.Range(0, count).Select(_ => array[random.Next(array.Length)]));

			//People.AddAsSorted(array[random.Next(array.Length)]);
			//People.Add(array[random.Next(array.Length)]);
			//People.AddRangeAsSorted(array);
		}

		public void ClearAll()
		{
			People.Clear();
		}
	}
}
