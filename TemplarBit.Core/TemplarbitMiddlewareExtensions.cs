using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplarBit.Core
{
    public static class TemplarBitMiddlewareExtensions
    {
        public static IApplicationBuilder UseTemplarBit(
            this IApplicationBuilder builder, TemplarBitMiddlewareModel model, ITemplarBitLogger logger)
        {
            return builder.UseMiddleware<TemplarBitMiddleware>(model, logger);
        }
    }
}
