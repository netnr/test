using System;
using System.Collections.Generic;

namespace Netnr.AuthFailed.Models;

public partial class BaseUser
{
    public long UserId { get; set; }

    public DateTime CreateTime { get; set; }

    public string UserAccount { get; set; }

    public string UserPassword { get; set; }

    public string UserNickname { get; set; }
}
