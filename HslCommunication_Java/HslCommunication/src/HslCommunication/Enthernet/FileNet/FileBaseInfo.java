package HslCommunication.Enthernet.FileNet;

public class FileBaseInfo {

    /**
     * 获取文件的名称
     * @return
     */
    public String getName() {
        return Name;
    }

    /**
     * 设置文件的名称
     * @param name
     */
    public void setName(String name) {
        Name = name;
    }

    /**
     * 获取文件的大小
     * @return
     */
    public long getSize() {
        return Size;
    }

    /**
     * 设置文件的大小
     * @param size
     */
    public void setSize(long size) {
        Size = size;
    }

    /**
     * 获取文件的标识，注释
     * @return
     */
    public String getTag() {
        return Tag;
    }

    /**
     * 设置文件的标识，注释
     * @param tag
     */
    public void setTag(String tag) {
        Tag = tag;
    }

    /**
     * 获取文件的上传人
     * @return
     */
    public String getUpload() {
        return Upload;
    }

    /**
     * 设置文件的上传人
     * @param upload
     */
    public void setUpload(String upload) {
        Upload = upload;
    }

    private String Name ;     // 文件名称
    private long Size;        // 文件大小
    private String Tag;       // 文件的标识，注释
    private String Upload;    // 文件上传人
}
