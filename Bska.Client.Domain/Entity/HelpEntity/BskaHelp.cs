
namespace Bska.Client.Domain.Entity.HelpEntity
{
    using Bska.Client.API.EF6;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class BskaHelp:Entity
    {
        public Int32 Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte[] Photo { get; set; }
        public string FileType { get; set; }
        public string TableItems { get; set; }
        public DateTime InsertDate { get; set; }
        public string Identity { get; set; }
    }
}
