using Core.SharedKernel.Domain;

namespace NotificationManagement.Domain.Entities
{
	public class EmailTemplate : AggregateRoot
	{
		public string Name { get; private set; } = string.Empty;
		public string Title { get; private set; } = string.Empty;
		public string Subject { get; private set; } = string.Empty;
		public string Body { get; private set; } = string.Empty;
		public List<string> Variables { get; private set; } = [];

		private EmailTemplate() { }

		public void Update(string title, string subject, string body)
		{
			if(string.IsNullOrEmpty(title) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(body))
			{
				throw new ArgumentException("Title, subject and body of the email are required.");
			}
			Title = title;
			Subject = subject;
			Body = body;
		}

		public void AddPlaceHolder(string tag)
		{
			Variables.Add(tag);
		}

		//public string Render(Dictionary<string, string> values)
		//{

		//}
	}
}
