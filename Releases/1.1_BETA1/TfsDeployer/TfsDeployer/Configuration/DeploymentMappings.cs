﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.112
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;
using System.Collections.Generic;

// 
// This source code was auto-generated by xsd, Version=2.0.50727.42.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.readify.net/TFSDeployer/DeploymentMappings20061026", IsNullable=false)]
public partial class DeploymentMappings {
    
    private List<Mapping> itemsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Mapping", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public List<Mapping> Mappings
    {
        get {
            return this.itemsField;
        }
        set {
            this.itemsField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class Mapping {
    
    private List<ScriptParameter> scriptParametersField;
    
    private string computerField;
    
    private string originalQualityField;
    
    private string newQualityField;

    private string scriptField;

    private string notificationAddressField;

    private string permittedUsersField;

    private RunnerType runnerTypeField;
    
    private bool runnerTypeFieldSpecified;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("ScriptParameter", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public List<ScriptParameter> ScriptParameters
    {
        get {
            return this.scriptParametersField;
        }
        set {
            this.scriptParametersField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Computer {
        get {
            return this.computerField;
        }
        set {
            this.computerField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string OriginalQuality {
        get {
            return this.originalQualityField;
        }
        set {
            this.originalQualityField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string NewQuality {
        get {
            return this.newQualityField;
        }
        set {
            this.newQualityField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Script
    {
        get
        {
            return this.scriptField;
        }
        set
        {
            this.scriptField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string NotificationAddress
    {
        get
        {
            return this.notificationAddressField;
        }
        set
        {
            this.notificationAddressField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string PermittedUsers
    {
        get
        {
            return this.permittedUsersField;
        }
        set
        {
            this.permittedUsersField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public RunnerType RunnerType {
        get {
            return this.runnerTypeField;
        }
        set {
            this.runnerTypeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool RunnerTypeSpecified {
        get {
            return this.runnerTypeFieldSpecified;
        }
        set {
            this.runnerTypeFieldSpecified = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class ScriptParameter {
    
    private string nameField;
    
    private string valueField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string value {
        get {
            return this.valueField;
        }
        set {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.readify.net/TFSDeployer/DeploymentMappings20061026")]
public enum RunnerType {
    
    /// <remarks/>
    PowerShell,
    
    /// <remarks/>
    BatchFile,
}
