namespace KongoCache.Core.DTOs
{
    public enum OpType
    {
        ADD,
        GET,
        REMOVE,

        HGET,
        HADD,
        HREMOVE,
        HGETALL,
        HREMOVEKEY,

        SGET_MEMBERS,
        SADD,
        SIS_MEMBER,
        SREMOVE_MEMBER,

        ALPUSH,
        ALPOP,
        ALRANGE,

        LLPUSH,
        LLPOP,
        LLGET_INDEX,
        LLADD,
               
        MAXSGET_MEMBERS,
        MAXSADD,
        MAXSIS_MEMBER,
        MAXSREMOVE_MEMBER,
        MAXS_TOP,

        MINSGET_MEMBERS,
        MINSADD,
        MINSIS_MEMBER,
        MINSREMOVE_MEMBER,
        MINS_TOP,

        INVALID

    }
}
