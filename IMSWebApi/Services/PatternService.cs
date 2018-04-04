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
    public class PatternService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;

        public PatternService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMPattern> getPatterns(int pageSize, int page, string search)
        {
            List<VMPattern> patternListView;
            var result = repo.MstPatterns.Where(p => !string.IsNullOrEmpty(search)
                    ? p.name.StartsWith(search)
                    || p.fabricHeight.ToString().StartsWith(search)
                    || p.liningHeight.ToString().StartsWith(search)
                    || p.meterPerInch.ToString().StartsWith(search)
                    || p.widthPerInch.ToString().StartsWith(search)
                    || p.setRateForPattern.ToString().StartsWith(search) : true)
                    .OrderByDescending(o => o.id)
                    .Skip(page * pageSize).Take(pageSize).ToList();

            patternListView = Mapper.Map<List<MstPattern>, List<VMPattern>>(result);

            return new ListResult<VMPattern>
            {
                Data = patternListView,
                TotalCount = repo.MstPatterns.Where(p => !string.IsNullOrEmpty(search)
                    ? p.name.StartsWith(search)
                    || p.fabricHeight.ToString().StartsWith(search)
                    || p.liningHeight.ToString().StartsWith(search)
                    || p.meterPerInch.ToString().StartsWith(search)
                    || p.widthPerInch.ToString().StartsWith(search)
                    || p.setRateForPattern.ToString().StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public List<VMLookUpItem> getPatternLookup()
        {
            return repo.MstPatterns.OrderBy(p => p.name)
                .Select(s => new VMLookUpItem
                {
                    value = s.id,
                    label = s.name
                }).ToList();
        }

        public VMPattern getPatternById(Int64 id)
        {
            var result = repo.MstPatterns.Where(p => p.id == id).FirstOrDefault();

            VMPattern patternView = Mapper.Map<MstPattern, VMPattern>(result);
            return patternView;
        }

        public ResponseMessage postPattern(VMPattern pattern)
        {
            MstPattern patternToPost = Mapper.Map<VMPattern, MstPattern>(pattern);
            patternToPost.createdOn = DateTime.Now;
            patternToPost.createdBy = _LoggedInuserId;

            repo.MstPatterns.Add(patternToPost);
            repo.SaveChanges();
            return new ResponseMessage(patternToPost.id, resourceManager.GetString("PatternAdded"), ResponseType.Success);
        }

        public ResponseMessage putPattern(VMPattern pattern)
        {
            var patternToPut = repo.MstPatterns.Where(p => p.id == pattern.id).FirstOrDefault();

            patternToPut.name = pattern.name;
            patternToPut.fabricHeight = pattern.fabricHeight;
            patternToPut.liningHeight = pattern.liningHeight;
            patternToPut.meterPerInch = pattern.meterPerInch;
            patternToPut.widthPerInch = pattern.widthPerInch;
            patternToPut.setRateForPattern = pattern.setRateForPattern;
            patternToPut.horizontalPatch = pattern.horizontalPatch;
            patternToPut.verticalPatch = pattern.verticalPatch;

            patternToPut.updatedBy = _LoggedInuserId;
            patternToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(pattern.id, resourceManager.GetString("PatternUpdated"), ResponseType.Success);
        }

        public ResponseMessage deletePattern(Int64 id)
        {
            repo.MstPatterns.Remove(repo.MstPatterns.Where(p => p.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("PatternDeleted"), ResponseType.Success);
        }
    }
}