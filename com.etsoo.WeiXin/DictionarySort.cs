namespace com.etsoo.WeiXin
{
    /// <summary>
    /// 字典排序
    /// </summary>
    public class DictionarySort : IComparer<string>
    {
        public int Compare(string? left, string? right)
        {
            var leftLen = left?.Length ?? 0;
            var rightLen = right?.Length ?? 0;
            var index = 0;
            while (index < leftLen && index < rightLen)
            {
                if (left![index] < right![index])
                    return -1;
                else if (left[index] > right[index])
                    return 1;
                else
                    index++;
            }
            return leftLen - rightLen;
        }
    }
}
