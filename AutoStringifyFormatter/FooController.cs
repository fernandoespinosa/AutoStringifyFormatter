using System;
using System.Web.Http;
using AutoStringifyFormatter.Models;
using Newtonsoft.Json;

namespace AutoStringifyFormatter
{
    [RoutePrefix("")]
    public class FooController : ApiController
    {
        [HttpGet]
        [Route]
        public Foo Get()
        {
            return new Foo
            {
                Guid = Guid.NewGuid(),
                BarJson = JsonConvert.SerializeObject(new Bar
                {
                    Guid = Guid.NewGuid(),
                    Baz = new Baz { Guid = Guid.NewGuid() }
                })
            };
        }

        [HttpPost]
        [Route]
        public void Get(Foo foo)
        {
            var bar = JsonConvert.DeserializeObject<Bar>(foo.BarJson);
        }
    }
}
