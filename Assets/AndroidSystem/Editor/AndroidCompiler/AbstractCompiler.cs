//------------------------------------------------------------------------------
// <auto-generated>
//     O código foi gerado por uma ferramenta.
//     Versão de Tempo de Execução:4.0.30319.34014
//
//     As alterações ao arquivo poderão causar comportamento incorreto e serão perdidas se
//     o código for gerado novamente.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.IO;
using System.Collections.Generic;

public abstract class AbstractCompiler
{
	public AbstractCompiler ()
	{

	}

	/// <summary>
	/// Configures the path.
	/// </summary>
	/// <returns><c>true</c>, if path was configured, <c>false</c> otherwise.</returns>
	public abstract bool ConfigurePath ();

	/// <summary>
	/// Generates the package.
	/// </summary>
	public abstract void GeneratePackage (bool updateAtFinish, Action<bool> completionCallback);

	/// <summary>
	/// Cleanups temporary files for package generation.
	/// </summary>
	public abstract void CleanupFiles (List<string> files);
}