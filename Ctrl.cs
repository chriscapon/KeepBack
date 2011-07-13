/*
    Copyright 2008,2009,2010,2011 Chris Capon.

    This file is part of KeepBack.

    KeepBack is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    KeepBack is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with KeepBack.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace KeepBack
{
	[XmlRoot(ElementName="keepback",Namespace="http://keepback.org/2008-09/keepback")]
	public class Ctrl
	{
		//--- field -----------------------------

		string          path      = null;
		CtrlArchive[]   archives  = null;

		//--- property --------------------------

		[XmlIgnore]
		public string          Path      { get { return path; } set { path = value; } }

		[XmlArray(ElementName="archives")] [XmlArrayItem(ElementName="archive")]
		public CtrlArchive[]   Archives  { get { return archives; } set { archives = value; } }

		//--- constructor -----------------------

		public Ctrl()
		{
		}

		//--- method ----------------------------

		public static string RelativePath( string root, string path )
		{
			if( string.IsNullOrEmpty( path ) )
			{
				return root;
			}
			return (string.IsNullOrEmpty( root ) || System.IO.Path.IsPathRooted( path )) ? path : System.IO.Path.Combine( root, path );
		}

		public static Ctrl Import( string filename )
		{
			filename = System.IO.Path.GetFullPath( filename );
			if( ! File.Exists( filename ) )
			{
				throw new ApplicationException( "File not found: " + filename );
			}
			try
			{
				XmlSerializer x = new XmlSerializer( typeof(Ctrl) );
				using( XmlReader r = XmlReader.Create( filename ) )
				{
					Ctrl c = (Ctrl)x.Deserialize( r );
					c.Path = filename;
					return c;
				}
			}
			catch( Exception ex )
			{
				throw new ApplicationException( "Control file: " + filename, ex );
			}
		}

		public void Export( string filename )
		{
			try
			{
				XmlSerializer x = new XmlSerializer( typeof(Ctrl) );
				XmlWriterSettings s = new XmlWriterSettings();
				s.Indent = true;
				using( XmlWriter w = XmlWriter.Create( filename, s ) )
				{
					x.Serialize( w, this );
				}
			}
			catch( Exception ex )
			{
				throw new ApplicationException( "Control file: " + filename, ex );
			}
		}

		public void ArchiveSort()
		{
			if( archives != null )
			{
				Array.Sort<CtrlArchive>( archives );
			}
		}
		public CtrlArchive ArchiveAdd()
		{
			CtrlArchive a = new CtrlArchive();
			List<CtrlArchive> list = (archives == null) ? new List<CtrlArchive>() : new List<CtrlArchive>( archives );
			list.Add( a );
			archives = list.ToArray();
			return a;
		}
		public void ArchiveDelete( CtrlArchive archive )
		{
			if( archives != null )
			{
				List<CtrlArchive> list = new List<CtrlArchive>( archives );
				list.Remove( archive );
				archives = (list.Count > 0) ? list.ToArray() : null;
			}
		}

		//--- end -------------------------------
	}
}
