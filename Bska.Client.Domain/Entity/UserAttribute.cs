
namespace Bska.Client.Domain.Entity
{
    using Bska.Client.API.EF6;
    using System;
    public class UserAttribute : Entity
    {
        public Int32 UserId { get; set; }
        public Boolean CanRequestUnConsum { get; set; }
        public Boolean CanRequestConsum { get; set; }
        public Boolean CanRequestInConsum { get; set; }
        public Boolean InternalRequest { get; set; }
        public Boolean SurplusRequest { get; set; }
        public Boolean RequestPrint { get; set; }
        public Boolean RecivedRequestPrint { get; set; }
        public Boolean ProceedingRequest { get; set; }
        public Boolean InternalMovedRequest { get; set; }
        public Boolean CanRequestBelonging { get; set; }
        public Boolean CanRequestInstallable { get; set; }
        public Boolean CanChangePassword { get; set; }
        public Boolean CanEditTrenderRequest { get; set; }

        //
        public Boolean Atttribute1 { get; set; }
        public Boolean Atttribute2 { get; set; }
        public Boolean Atttribute3 { get; set; }
        public Boolean Atttribute4 { get; set; }
        public Boolean Atttribute5 { get; set; }
        public Boolean Atttribute6 { get; set; }
        public Boolean Atttribute7 { get; set; }
        public Boolean Atttribute8 { get; set; }
        public Boolean Atttribute9 { get; set; }
        public Boolean Atttribute10 { get; set; }
        public Boolean Atttribute11 { get; set; }

        public virtual Users User { get; set; }
    }
}
