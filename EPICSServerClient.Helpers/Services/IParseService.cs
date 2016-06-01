﻿using EPICSServerClient.Helpers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EPICSServerClient.Helpers.Services
{
    public interface IParseService
    {
        List<TabItem> GetClasses();
        List<PFObject> GetClassObjects(string Class);
        string PostClassObject(string Class, PFObject obj);
        void DeleteClassObject(string Class, PFObject obj);
    }
}
