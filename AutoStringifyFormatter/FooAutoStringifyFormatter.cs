using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoStringifyFormatter.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AutoStringifyFormatter
{
    public class FooAutoStringifyFormatter : MediaTypeFormatter
    {
        public override bool CanReadType(Type type)
        {
            return type == typeof(Foo);
        }

        public override bool CanWriteType(Type type)
        {
            return type == typeof(Foo);
        }

        public FooAutoStringifyFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var streamReader = new StreamReader(readStream);
            var jsonReader = new JsonTextReader(streamReader);

            var foo = new Foo();
            var bar = new Bar {Baz = new Baz()};

            while (jsonReader.Read())
            {
                if (jsonReader.TokenType == JsonToken.PropertyName)
                {
                    jsonReader.Read();
                    var value = (string) jsonReader.Value;
                    switch (jsonReader.Path)
                    {
                        case "Guid":
                            foo.Guid = Guid.Parse(value);
                            break;
                        case "Bar.Guid":
                            bar.Guid = Guid.Parse(value);
                            break;
                        case "Bar.Baz.Guid":
                            bar.Baz.Guid = Guid.Parse(value);
                            break;
                    }
                }
            }

            foo.BarJson = JsonConvert.SerializeObject(bar);

            return Task.FromResult((object)foo);
        }


        public async Task<object> ReadFromStreamAsync_Alt(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var streamReader = new StreamReader(readStream);
            dynamic foo = JObject.Parse(await streamReader.ReadToEndAsync());

            return new Foo
            {
                Guid = foo.Guid,
                BarJson = JsonConvert.SerializeObject(foo.Bar)
            };
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            var foo = (Foo)value;
            var streamWriter = new StreamWriter(writeStream);
            var jsonWriter = new JsonTextWriter(streamWriter) { Formatting = Formatting.Indented };

            jsonWriter.WriteStartObject();

            jsonWriter.WritePropertyName("Guid");
            jsonWriter.WriteValue(foo.Guid);

            jsonWriter.WritePropertyName("Bar");
            jsonWriter.WriteRawValue(foo.BarJson);

            jsonWriter.WriteEndObject();
            jsonWriter.Flush();

            return Task.FromResult(0);
        }
    }
}