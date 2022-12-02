using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public interface IDbContextBuilder
    {
        DbContextOptions GetContextOptions();
    }
}
