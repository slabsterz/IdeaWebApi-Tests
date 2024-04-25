using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaCenterApi.Models
{
    public interface IBaseIdeaModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }
    }
}
