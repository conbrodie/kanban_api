using System;
using api.Models;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class ExampleSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(Sprint))
        {
            schema.Example = new OpenApiObject()
            {
                ["projectId"] = new OpenApiInteger(0),
                ["sprintName"] = new OpenApiString("Sprint name"),
            };
        }

        if (context.Type == typeof(SprintList))
        {
            schema.Example = new OpenApiObject()
            {
                ["sprintId"] = new OpenApiInteger(0),
                ["name"] = new OpenApiString("Sprint list name"),
            };
        }

        if (context.Type == typeof(Project))
        {
            schema.Example = new OpenApiObject()
            {
                ["title"] = new OpenApiString("Project name"),
                ["startDate"] = new OpenApiDateTime(new DateTime(2000, 01, 01)),
                ["endDate"] = new OpenApiDateTime(new DateTime(2000, 01, 01)),
                ["approved"] = new OpenApiBoolean(false),
            };
        }

         if (context.Type == typeof(Card))
        {
            schema.Example = new OpenApiObject()
            {
                ["sprintListId"] = new OpenApiInteger(0),
                ["cardTitle"] = new OpenApiString("string"),
                ["cardDescription"] = new OpenApiString("string"),
                ["cardPriority"] = new OpenApiString("string"),
                ["cardDateCreated"] = new OpenApiDateTime(new DateTime(2000, 01, 01)),
                ["cardDateDue"] = new OpenApiDateTime(new DateTime(2000, 01, 01)),
            };
        }

        if (context.Type == typeof(Department))
        {
            schema.Example = new OpenApiObject()
            {
                ["departmentName"] = new OpenApiString("string"),
                ["departmentDescription"] = new OpenApiString("string"),
                ["color"] = new OpenApiString("#000")
            };
        }

        if (context.Type == typeof(DepartmentMember))
        {
            schema.Example = new OpenApiObject()
            {
                ["userId"] = new OpenApiInteger(0),
            };
        }
    }
}