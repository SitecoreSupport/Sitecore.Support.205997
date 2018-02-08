using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sitecore.Framework.Rules;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;

namespace Sitecore.Support.XConnect.Segmentation.Predicates.Interactions
{
  public class PageOrDescendantsVisitMatches : ICondition
  {
    public Guid ItemId { get; set; }

    [SuppressMessage("Data Flow", "SC1062:ValidateArgumentsOfPublicMethods", MessageId = "0#")]
    public bool Evaluate(IRuleExecutionContext context)
    {
      var contact = context.Fact<Contact>();
      if (contact.Interactions == null) return false;
      return contact.Interactions.Any(i => i.Events.OfType<PageViewEvent>().Any(e => e.ItemId == this.ItemId)) || contact.Interactions.SelectMany(i => i.Events.OfType<PageViewEvent>()).Select(i => i.ItemId).Any(CheckItem);
    }

    private bool CheckItem(Guid id)
    {
      var item = Configuration.Factory.GetDatabase("master").GetItem(id.ToString());
      var parent = item?.Parent;

      if (parent == null) return false;
      while (parent != null)
      {
        if (parent.ID.ToGuid() == this.ItemId)
        {
          return true;
        }
        parent = parent.Parent;
      }
      return false;
    }
  }
}
