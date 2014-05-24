== YACL

		Yet Another Command Line

== Getting Started

1. Create a new C# console application

2. Add a reference to YACL

3. Derive your class with Main in it from YACL.ApplicationBase

4. In your Main, do the following:

		var app = new Program();
		app.Start(args);

5. Anywhere in the same assembly with Main, create classes that derive from YACL.CommandBase

6. Here's an example implementation of a YACL Command

		using System;
		using System.Text;
		using YACL;

		namespace drep4.Commands
		{
		    public class FooCommand : CommandBase
		    {
		        public override string Name
		        {
		            get { return "foo"; }
		        }

		        public override string Description
		        {
		            get { return "runs the foo."; }
		        }

		        public override bool RequiresArguments
		        {
		            get { return true; }
		        }

		        public override string[] RequiredArguments
		        {
		            get
		            {
		                return new[]
		                {
		                    "arg1", "arg2"
		                };
		            }
		        }

		        public override string Usage
		        {
		            get
		            {
		                var usage = new StringBuilder();
		                usage.AppendFormat("drep4.exe foo arg1=value arg2=value");
		                return usage.ToString();
		            }
		        }

		        public override void Execute()
		        {
		            foreach (var kvp in ParsedArguments)
		            {
		                Console.WriteLine("{0}:{1}", kvp.Key, kvp.Value);
		            }
		        }
		    }
		}
		
7. Then to use this command on the command line, simply run your application with arguments like this:

		app.exe foo arg1=hello arg2=world