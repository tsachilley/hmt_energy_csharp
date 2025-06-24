namespace hmt_energy_csharp.VDRs
{
    public class TransDto
    {
        public float? history_currenttime { get; set; }
        public float? history_dpt_depth { get; set; }
        public float? history_dpt_offset { get; set; }
        public double history_gns_longtitude { get; set; }
        public double history_gns_latitude { get; set; }
        public string history_gns_satnum { get; set; }
        public string history_gns_antennaaltitude { get; set; }
        public float? history_mwd_tdirection { get; set; }
        public float? history_mwd_magdirection { get; set; }
        public float? history_mwd_knspeed { get; set; }
        public float? history_mwd_speed { get; set; }
        public string history_rpm_source { get; set; }
        public string history_rpm_number { get; set; }
        public float? history_rpm_speed { get; set; }
        public float? history_rpm_propellerpitch { get; set; }
        public float? history_vbw_watspd { get; set; }
        public float? history_vbw_grdspd { get; set; }
        public float? history_vtg_grdcoztrue { get; set; }
        public float? history_vtg_grdcozmag { get; set; }
        public float? history_vtg_grdspdknot { get; set; }
        public float? history_vtg_grdspdkm { get; set; }
        public float? history_vlw_watdistotal { get; set; }
        public float? history_vlw_watdisreset { get; set; }
        public float? history_vlw_grddistotal { get; set; }
        public float? history_vlw_grddisreset { get; set; }
        public float? history_draft_trim { get; set; }
        public float? history_draft_heel { get; set; }
        public float? history_draft_draft { get; set; }
        public float? history_power_rpm { get; set; }
        public float? history_power_power { get; set; }
        public float? history_power_slip { get; set; }
        public float? history_flowmeter_me_fcpernm { get; set; }
        public float? history_flowmeter_me_fcperpow { get; set; }
        public float? history_flowmeter_fcpernm { get; set; }
        public float? history_flowmeter_fcperpow { get; set; }
        public float? history_dg_power { get; set; }
        public float? history_draft_bow { get; set; }
        public float? history_draft_astern { get; set; }
        public float? history_draft_port { get; set; }
        public float? history_draft_starboard { get; set; }
        public float? history_mefc { get; set; }
        public float? history_meacc { get; set; }
        public float? history_dgfc { get; set; }
        public float? history_dgacc { get; set; }
        public float? history_blrfc { get; set; }
        public float? history_blracc { get; set; }
    }
}