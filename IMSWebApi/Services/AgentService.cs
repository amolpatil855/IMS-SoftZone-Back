using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using Microsoft.AspNet.Identity;
using IMSWebApi.Common;
using IMSWebApi.ViewModel;
using AutoMapper;
using IMSWebApi.Enums;

namespace IMSWebApi.Services
{
    public class AgentService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        public AgentService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMAgent> getAgent(int pageSize, int page, string search)
        {
            List<VMAgent> agentView;
            if (pageSize > 0)
            {
                var result = repo.MstAgents.Where(a => !string.IsNullOrEmpty(search)
                    ? a.email.StartsWith(search)
                    || a.phone.StartsWith(search) : true)
                    .OrderBy(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                agentView = Mapper.Map<List<MstAgent>, List<VMAgent>>(result);
            }
            else
            {
                var result = repo.MstAgents.Where(a => !string.IsNullOrEmpty(search)
                    ? a.email.StartsWith(search)
                    || a.phone.StartsWith(search) : true).ToList();
                agentView = Mapper.Map<List<MstAgent>, List<VMAgent>>(result);
            }

            return new ListResult<VMAgent>
            {
                Data = agentView,
                TotalCount = repo.MstAgents.Where(a => !string.IsNullOrEmpty(search)
                    ? a.email.StartsWith(search)
                    || a.phone.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMAgent getAgentById(Int64 id)
        {
            var result = repo.MstAgents.Where(s => s.id == id).FirstOrDefault();
            VMAgent agentView = Mapper.Map<MstAgent, VMAgent>(result);
            return agentView;
        }

        public List<VMLookUpItem> getAgentLookup()
        {
            return repo.MstAgents.OrderBy(s => s.name)
                .Select(s => new VMLookUpItem
                {
                    value = s.id,
                    label = s.name 
                }).ToList();
        }

        public ResponseMessage postAgent(VMAgent agent)
        {
            MstAgent agentToPost = Mapper.Map<VMAgent, MstAgent>(agent);
            agentToPost.createdOn = DateTime.Now;
            agentToPost.createdBy = _LoggedInuserId;

            repo.MstAgents.Add(agentToPost);
            repo.SaveChanges();
            return new ResponseMessage(agentToPost.id,
                resourceManager.GetString("AgentAdded"), ResponseType.Success);
        }

        public ResponseMessage putAgent(VMAgent agent)
        {
            var agentToPut = repo.MstAgents.Where(s => s.id == agent.id).FirstOrDefault();
            agentToPut.name = agent.name;
            agentToPut.phone = agent.phone;
            agentToPut.email = agent.email;
            agentToPut.address1 = agent.address1;
            agentToPut.address2 = agent.address2;
            agentToPut.city = agent.city;
            agentToPut.state = agent.state;
            agentToPut.pin = agent.pin;
            agentToPut.commision = agent.commision;
            agentToPut.updatedBy = _LoggedInuserId;
            agentToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(agent.id, resourceManager.GetString("AgentUpdated"), ResponseType.Success);
        }

        public ResponseMessage deleteAgent(Int64 id)
        {
            repo.MstAgents.Remove(repo.MstAgents.Where(s => s.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("AgentDeleted"), ResponseType.Success);
        }
    }
}