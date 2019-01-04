public class Mail
{
    public int Id { get; set; }
    public string MailText { get; set; }

    public virtual User User { get; set; }
}