import xml.etree.ElementTree as ET
from sphinx.directives import ObjectDescription
from sphinx.domains import Domain, ObjType, Index
from sphinx.util.docfields import Field, TypedField, GroupedField
from sphinx.locale import l_, _
from sphinx import addnodes
from sphinx.util.nodes import clean_astext, make_refnode
from sphinx.util import ws_re
from docutils.parsers.rst import directives, states
from docutils import nodes
from sphinx.roles import XRefRole
import os
import textwrap
import json
import sys

dotnet_docs = None

class DotNetObject(ObjectDescription):
  
  display_prefix = None
  
  option_spec = {
    'name': directives.unchanged,
    'fullname': directives.unchanged,
    'namespace': directives.unchanged,
    'inherits': directives.unchanged,
    'implements': directives.unchanged,
  }

  def handle_signature(self, sig, signode):
    if self.display_prefix:
      signode += addnodes.desc_annotation(self.display_prefix,
                                          self.display_prefix)
      
    name = self.options['name']
    namespace = self.options['namespace']
    fullname = self.options['fullname']
    inherits = json.loads(self.options['inherits']) if 'inherits' in self.options else ("","")
    implements = json.loads(self.options['implements']) if 'implements' in self.options else []
      
    signode += addnodes.desc_name(name, name)
    
    toappend = []
    
    if inherits[0] != "Object" and inherits[0] != "Enum" and inherits[0] != "":
      xref = addnodes.pending_xref(
        ':ref:`' + inherits[1] + '`',
        refdomain='std',
        reftype='ref',
        reftarget=ws_re.sub(' ', inherits[1].lower()),
        refexplicit=False)
      xref += nodes.Text(inherits[0], inherits[0])
      toappend.append(xref)
    
    for implement in implements:
      if len(toappend) > 0:
        toappend.append(nodes.Text(', ', u',\xa0'))
      xref = addnodes.pending_xref(
        ':ref:`' + implement[1] + '`',
        refdomain='std',
        reftype='ref',
        reftarget=ws_re.sub(' ', implement[1].lower()),
        refexplicit=False)
      xref += nodes.Text(implement[0], implement[0])
      toappend.append(xref)
    
    if len(toappend) > 0:
      signode += nodes.Text(' : ', u'\xa0:\xa0')
      for a in toappend:
        signode += a
    
    return name, namespace

class DotNetClass(DotNetObject):
  display_prefix = 'class '
  
  doc_field_types = [
    GroupedField('typeparameters', label=l_('Type Parameters'),
                 rolename='typeparam', names=('typeparam', 'typeparameter')),
  ]

class DotNetStruct(DotNetObject):
  display_prefix = 'struct '
  
  doc_field_types = [
    GroupedField('typeparameters', label=l_('Type Parameters'),
                 rolename='typeparam', names=('typeparam', 'typeparameter')),
  ]
  
class DotNetInterface(DotNetObject):
  display_prefix = 'interface '
  
  doc_field_types = [
    GroupedField('typeparameters', label=l_('Type Parameters'),
                 rolename='typeparam', names=('typeparam', 'typeparameter')),
  ]

class DotNetEnum(DotNetObject):
  display_prefix = 'enum '

class DotNetMethod(DotNetObject):
  
  option_spec = {
    'name': directives.unchanged,
    'prefix': directives.unchanged,
    'parameters': directives.unchanged,
    'return': directives.unchanged,
  }
  
  doc_field_types = [
    TypedField('parameter', label=l_('Parameters'),
                names=('param', 'parameter', 'arg', 'argument'),
                typerolename='type', typenames=('type',)),
    GroupedField('typeparameters', label=l_('Type Parameters'),
                 rolename='typeparam', names=('typeparam', 'typeparameter')),
  ]

  def handle_signature(self, sig, signode):
    name = self.options['name']
    parameters = json.loads(self.options['parameters']) if 'parameters' in self.options else []
    return_v = json.loads(self.options['return']) if 'return' in self.options else ("","")
    prefix = self.options['prefix'] if 'prefix' in self.options else ""
  
    if not ('parameters' in self.options) or not ('return' in self.options):
      print("WARNING: No parameters or return set for '" + name + "'")
    
    signode += nodes.Text(prefix + ' ', prefix + u'\xa0')
    xref = addnodes.pending_xref(
      ':ref:`' + return_v[1] + '`',
      refdomain='std',
      reftype='ref',
      reftarget=ws_re.sub(' ', return_v[1].lower()),
      refexplicit=False)
    xref += nodes.Text(return_v[0], return_v[0])
    signode += xref
    signode += nodes.Text(' ', u'\xa0')
      
    signode += addnodes.desc_name(name, name)
    paramlist = addnodes.desc_parameterlist()
    for tn in parameters:
      param = addnodes.desc_parameter('', '', noemph=True)
      prefix = ''
      if len(tn) > 3 and tn[3] == "True":
        prefix += "ref "
      if len(tn) > 4 and tn[4] == "True":
        prefix += "out "
      
      if prefix != "":
        param += nodes.Text(prefix + ' ', prefix + u'\xa0')
        
      xref = addnodes.pending_xref(
        ':ref:`' + tn[2] + '`',
        refdomain='std',
        reftype='ref',
        reftarget=ws_re.sub(' ', tn[2].lower()),
        refexplicit=False)
      xref += nodes.Text(tn[0], tn[0])
      param += xref
      param += nodes.emphasis(' '+tn[1], u'\xa0'+tn[1])
      paramlist += param
    signode += paramlist
    
    return name, ""

class DotNetProperty(DotNetObject):
  
  option_spec = {
    'name': directives.unchanged,
    'prefix': directives.unchanged,
    'type': directives.unchanged,
  }
  
  doc_field_types = [
    Field('value', label=l_('Value'), has_arg=False,
          names=('value', 'value')),
  ]

  def handle_signature(self, sig, signode):
    name = self.options['name']
    prefix = self.options['prefix']
    type_v = json.loads(self.options['type'])
    
    if prefix != 'readwrite':
      signode += nodes.Text(prefix, prefix)
      signode += nodes.Text(' ', u'\xa0')
    
    xref = addnodes.pending_xref(
      ':ref:`' + type_v[1] + '`',
      refdomain='std',
      reftype='ref',
      reftarget=ws_re.sub(' ', type_v[1].lower()),
      refexplicit=False)
    xref += nodes.Text(type_v[0], type_v[0])
    signode += xref
    signode += nodes.Text(' ', u'\xa0')
      
    signode += addnodes.desc_name(name, name)
    
    return name, ""

class DotNetField(DotNetObject):
  
  option_spec = {
    'name': directives.unchanged,
    'type': directives.unchanged,
  }
  
  doc_field_types = [
    Field('value', label=l_('Value'), has_arg=False,
          names=('value', 'value')),
  ]

  def handle_signature(self, sig, signode):
    name = self.options['name']
    type_v = json.loads(self.options['type'])
    
    xref = addnodes.pending_xref(
      ':ref:`' + type_v[1] + '`',
      refdomain='std',
      reftype='ref',
      reftarget=ws_re.sub(' ', type_v[1].lower()),
      refexplicit=False)
    xref += nodes.Text(type_v[0], type_v[0])
    signode += xref
    signode += nodes.Text(' ', u'\xa0')
      
    signode += addnodes.desc_name(name, name)
    
    return name, ""
  
class DotNetMethodRole(XRefRole):
  def process_link(self, env, refnode, has_explicit_title, title, target):
    paren_start = target.index("(")
    dot_start = target.rindex(".", 0, paren_start)
    dot_start = target.rindex(".", 0, dot_start)
    refnode['refdomain']='std'
    refnode['reftype']='ref'
    refnode['refexplicit']=True
    return (target[dot_start+1:paren_start], ws_re.sub(' ', target.lower()))
  
class DotNetNonMethodRole(XRefRole):
  def process_link(self, env, refnode, has_explicit_title, title, target):
    dot_start = target.rindex(".", 0)
    try:
      dot_start = target.rindex(".", 0, dot_start)
    except ValueError:
      dot_start = -1
    refnode['refdomain']='std'
    refnode['reftype']='ref'
    refnode['refexplicit']=True
    return (target[dot_start+1:], ws_re.sub(' ', target.lower()))

class DotNetDomain(Domain):
  name = 'dotnet'
  label = 'dotnet'

  object_types = {
    #'package':     ObjType(l_('package'), 'package', 'ref'),
    'class':        ObjType(l_('class'), 'class', 'ref'),
    'enum':        ObjType(l_('enum'), 'enum', 'ref'),
    'interface':        ObjType(l_('interface'), 'interface', 'ref'),
    'struct':        ObjType(l_('struct'), 'struct', 'ref'),
    #'field':       ObjType(l_('field'), 'field', 'ref'),
    #'constructor': ObjType(l_('constructor'), 'construct', 'ref'),
    'method':      ObjType(l_('method'), 'method', 'ref'),
    'nproperty':      ObjType(l_('property'), 'property', 'ref'),
    'nfield':      ObjType(l_('field'), 'field', 'ref'),
  }

  directives = {
    #'package':        JavaPackage,
    'class':          DotNetClass,
    'struct':          DotNetStruct,
    'interface':      DotNetInterface,
    'enum':          DotNetEnum,
    #'field':          JavaField,
    #'constructor':    JavaConstructor,
    'method':         DotNetMethod,
    'nproperty':         DotNetProperty,
    'nfield':         DotNetField,
    #'import':         JavaImport
  }

  roles = {
    #'package':   JavaXRefRole(),
    #'type':      JavaXRefRole(),
    #'field':     JavaXRefRole(),
    #'construct': JavaXRefRole(),
    'type':      DotNetNonMethodRole(warn_dangling=True),
    'method':      DotNetMethodRole(warn_dangling=True),
    #'ref':       JavaXRefRole(),
  }
  
def normalize_name_to_filename(basename):
  return basename.lower().replace(" ", "_").replace("`", "_").replace("<", "_").replace(">", "_")
  
def get_indented_rest(elem, name, indent = ""):
  if name == None:
    text = elem.text
  else:
    text = elem.find(name)
    if text == None:
      return None
    text = text.text
  if text == None:
    return None
  norm = textwrap.dedent(text)
  text = ("\n" + indent).join(norm.split("\n"))
  return indent + text.strip()
  
def convert_to_keyword_if_possible(name, alt=None):
  if name == "String":
    return "string"
  if name == "Single":
    return "float"
  if name == "Double":
    return "double"
  if name == "Int16":
    return "short"
  if name == "UInt16":
    return "ushort"
  if name == "Int32":
    return "int"
  if name == "UInt32":
    return "uint"
  if name == "Int64":
    return "long"
  if name == "UInt64":
    return "ulong"
  if name == "Decimal":
    return "decimal"
  if name == "Byte":
    return "byte"
  if name == "Char":
    return "char"
  if name == "Void":
    return "void"
  if name == "Boolean":
    return "bool"
  if alt != None:
    return alt
  return name
  
def generate_doc_at_path(app, elem, path):
  class_name = elem.get("Name")
  
  content = []
  
  content.append(".. _`" + elem.get("Anchor") + "`:")
  content.append("")
  content.append(elem.get("Name"))
  content.append("=============================================================")
  content.append("")
  content.append("""
.. raw:: html

    <style type="text/css">
      td.field-body {
        padding-top: 8px !important;
      }
      td.field-body > ul.first.last.simple {
        margin-top: -3px;
      }
    </style>
""")
  
  if elem.get("IsProtogameInternal") == "True":
    interface_ref = elem.get("InterfaceRef")
    if interface_ref == None:
      content.append(".. warning::")
      content.append("")
      content.append("    This documentation is for an internal class.  This class ")
      content.append("    is not intended to be used by developers, and is used ")
      content.append("    internally within the engine to provide functionality. ")
      content.append("")
      content.append("    Information documented here may not be up to date.")
      content.append("")
    else:
      content.append(".. warning::")
      content.append("")
      content.append("    This documentation is for an implementation of a service.")
      content.append("    This class is not meant to be used directly; instead use ")
      content.append("    the :ref:`" + elem.get("InterfaceRef") + "` service via ")
      content.append("    dependency injection to access this functionality.")
      content.append("")
      content.append("    Information documented here may not be up to date.")
      content.append("")
    
  i1 = "    "
  i2 = "        "
  i3 = "            "
  
  content.append(".. dotnet:" + elem.get("Type").lower() + ":: " + elem.get("FullName"))
  content.append(i1 + ":name: " + elem.get("Name"))
  content.append(i1 + ":fullname: " + elem.get("FullName"))
  content.append(i1 + ":namespace: " + elem.get("Namespace"))
  
  for inherit in elem.findall("Inherits"):
    content.append(i1 + ":inherits: " + json.dumps((
      convert_to_keyword_if_possible(inherit.get("TypeName"), inherit.get("TypeAnchor")),
      inherit.get("TypeAnchor"))))
    
  implements = []
  for implement in elem.findall("Implements"):
    implements.append((
      convert_to_keyword_if_possible(implement.get("TypeName"), implement.get("TypeAnchor")),
      implement.get("TypeAnchor")))
    
  content.append(i1 + ":implements: " + json.dumps(implements))
  
  content.append(i1)
  summary = get_indented_rest(elem, "Summary", i1)
  if summary != None:
    content.append(summary)
    content.append(i1)
    
  typeparams = elem.findall("TypeParameter")
  for typeparam in typeparams:
    paramdoc = get_indented_rest(typeparam, None, "")
    if paramdoc != None:
      if "\n" in paramdoc:
        paramdoc = "\n" + get_indented_rest(typeparam, None, i3)
    else:
      paramdoc = ""
    content.append(i1 + ":typeparam " + 
                   "`" + typeparam.get("TypeName") + "`: " + paramdoc)
  if len(typeparams) > 0:
    content.append(i1)
    
  for field in elem.findall("Field"):
    if field.get("IsPublic") != "True":
      continue
    
    content.append(i1 + ".. _`" + field.get("Anchor") + "`:")
    content.append(i1)
    content.append(i1 + ".. dotnet:nfield:: " + field.get("Name"))
    content.append(i2 + ":name: " + field.get("Name"))
    content.append(i2 + ":type: " + json.dumps((
      convert_to_keyword_if_possible(field.get("TypeName"), field.get("TypeAnchor")),
      field.get("TypeAnchor"))))
    content.append(i2)
    summary = get_indented_rest(field, "Summary", i2)
    if summary != None:
      content.append(summary)
      content.append(i2)
    valuedoc = get_indented_rest(field, "Value", "")
    if valuedoc != None:
      if "\n" in valuedoc:
        valuedoc = "\n" + get_indented_rest(field, "Value", i3)
      content.append(i2 + ":value: " + valuedoc)
    content.append(i1)
    
  for prop in elem.findall("Property"):
    if ((prop.get("HasGet") != "True") or (prop.get("HasGet") == "True" and prop.get("IsGetPublic") != "True") and
        (prop.get("HasSet") != "True") or (prop.get("HasSet") == "True" and prop.get("IsSetPublic") != "True")):
      continue
    
    prefix = "readwrite"
    readonly = prop.get("HasSet") == None or prop.get("IsSetPublic") != "True"
    writeonly = prop.get("HasGet") == None or prop.get("IsGetPublic") != "True"
    if readonly:
      prefix = "readonly"
    if writeonly:
      prefix = "writeonly"
    
    content.append(i1 + ".. _`" + prop.get("Anchor") + "`:")
    content.append(i1)
    content.append(i1 + ".. dotnet:nproperty:: " + prop.get("Name"))
    content.append(i2 + ":name: " + prop.get("Name"))
    content.append(i2 + ":prefix: " + prefix)
    content.append(i2 + ":type: " + json.dumps((
      convert_to_keyword_if_possible(prop.get("TypeName"), prop.get("TypeAnchor")),
      prop.get("TypeAnchor"))))
    content.append(i2)
    summary = get_indented_rest(prop, "Summary", i2)
    if summary != None:
      content.append(summary)
      content.append(i2)
    valuedoc = get_indented_rest(prop, "Value", "")
    if valuedoc != None:
      if "\n" in valuedoc:
        valuedoc = "\n" + get_indented_rest(prop, "Value", i3)
      content.append(i2 + ":value: " + valuedoc)
    content.append(i1)
    
  for method in elem.findall("Method"):
    if method.get("IsPrivate") == "True":
      continue
    if method.get("IsProtected") == "True" and method.get("IsAbstract") == "False":
      continue
    
    prefix = []
    if elem.get("Type").lower() == "class":
      if method.get("IsPrivate") == "True":
        prefix.append("private")
      if method.get("IsProtected") == "True":
        prefix.append("protected")
      if method.get("IsPublic") == "True":
        prefix.append("public")
      if method.get("IsAbstract") == "True":
        prefix.append("abstract")
    
    parameters = method.findall("Parameter")
    
    anchorparams = []
    for parameter in parameters:
      anchorparams.append(
        convert_to_keyword_if_possible(parameter.get("TypeName"), parameter.get("TypeAnchor")))
    anchorparams = ", ".join(anchorparams)
    
    content.append(i1 + ".. _`" + method.get("Anchor") + "`:")
    content.append(i1)
    content.append(i1 + ".. dotnet:method:: " + method.get("Name"))
    content.append(i2 + ":name: " + method.get("Name"))
    content.append(i2 + ":prefix: " + " ".join(prefix))
    
    parameters_for_json = []
    for parameter in parameters:
      parameters_for_json.append((
        convert_to_keyword_if_possible(parameter.get("TypeName"), parameter.get("TypeAnchor")),
        parameter.get("Name"),
        parameter.get("TypeAnchor"),
        parameter.get("IsRef"),
        parameter.get("IsOut")))
    
    content.append(i2 + ":parameters: " + json.dumps(parameters_for_json))
    content.append(i2 + ":return: " + json.dumps((
      convert_to_keyword_if_possible(method.get("ReturnTypeName"), method.get("ReturnTypeAnchor")),
      method.get("ReturnTypeAnchor"))))
    
    summary = get_indented_rest(method, "Summary", i2)
    if summary != None:
      content.append(i2)
      content.append(summary)
    content.append(i2)
    
    typeparams = method.findall("TypeParameter")
    for typeparam in typeparams:
      paramdoc = get_indented_rest(typeparam, None, "")
      if paramdoc != None:
        if "\n" in paramdoc:
          paramdoc = "\n" + get_indented_rest(typeparam, None, i3)
      else:
        paramdoc = ""
      content.append(i2 + ":typeparam " + 
                    "`" + typeparam.get("TypeName") + "`: " + paramdoc)
    
    for parameter in parameters:
      paramdoc = get_indented_rest(parameter, None, "")
      if paramdoc != None:
        if "\n" in paramdoc:
          paramdoc = "\n" + get_indented_rest(parameter, None, i3)
      else:
        paramdoc = ""
      prefix = ""
      if parameter.get("IsRef") == "True":
        prefix += "(ref) "
      if parameter.get("IsOut") == "True":
        prefix += "(out) "
      content.append(i2 + ":param " + 
                     convert_to_keyword_if_possible(parameter.get("TypeName"), parameter.get("TypeAnchor")) + " " + 
                     "`" + prefix + parameter.get("Name") + "`: " + paramdoc)
    if method.get("ReturnTypeFullName") != "System.Void":
      returndoc = get_indented_rest(method, "Returns", "")
      if returndoc != None:
        if "\n" in returndoc:
          returndoc = "\n" + get_indented_rest(method, "Returns", i3)
        content.append(i2 + ":returns: " + returndoc)
      
    content.append(i1)
  
  result = "\n".join(content)
  if os.path.exists(path):
    with open(path, 'r') as f:
      old_result = f.read()
  else:
    old_result = None
    
  if old_result != result:
    app.info("generating documentation for class... " + elem.get("FullName"))
    with open(path, 'w') as f:
      if sys.version_info >= (3, 0):
        f.write(result)
      else:
        f.write(result.encode('utf8'))
    
def process_type(app, elem):
  name = elem.get("Name")
  module = elem.get("Module")
  if elem.get("IsPublic") != "True":
    return
  
  module_name = normalize_name_to_filename(module)
  internal = elem.get("IsProtogameInternal") == "True"
      
  path = module_name + "/api"
  if internal:
    path = "_internal/" + path
  
  if not os.path.isdir(path):
    os.makedirs(path)
      
  path = path + "/" + normalize_name_to_filename(name) + ".gen"
  generate_doc_at_path(app, elem, path + ".rst")
  
def load_xml(app):
  global dotnet_docs
  import subprocess, os
  app.info("current working directory " + os.getcwd())
  if not os.path.isfile('../Protogame.Docs/Protogame.combined.xml'):
    app.info("running pre-build...")
    p = subprocess.Popen(['bash', 'build.sh'], cwd='../Protogame.Docs/_ext')
    p.wait()
  app.info("loading .net xml documentation")
  with open('../Protogame.Docs/Protogame.combined.xml', 'r') as content_file:
    content = content_file.read()
  dotnet_docs = ET.fromstring(content)
  app.info("loaded .net xml documentation")
  for elem in dotnet_docs.findall("./Type"):
    process_type(app, elem)
  
def setup(app):
  app.add_domain(DotNetDomain)
  
  app.connect('builder-inited', load_xml)
  
  return {'version': '0.1'}

