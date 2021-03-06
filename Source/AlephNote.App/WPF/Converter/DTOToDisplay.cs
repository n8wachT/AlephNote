﻿using AlephNote.WPF.MVVM;
using System;

namespace AlephNote.WPF.Converter
{
	class DTOToDisplay : OneWayConverter<DateTimeOffset, string>
	{
		protected override string Convert(DateTimeOffset value, object parameter)
		{
			return value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
		}
	}
}
